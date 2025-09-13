using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityForum.Domain.Exceptions;
using CommunityForum.Domain.Enums;
using CommunityForum.Application.Mappers;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _cut;
        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cut = new UserService(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object);
        }
        [Test]
        public void RegisterUserAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.RegisterUserAsync(null));
            Assert.That(exception.Message, Does.StartWith("Register user request can not be null."));
        }
        [Test]
        public void RegisterUserAsync_ThrowsUsernameAlreadyExistsException_IfUsernameExistsInDB()
        {
            var request = new RegisterUserRequest("username", "password", "email");
            _userRepositoryMock.Setup(mock => mock.GetByUsernameAsync(request.Username))
                .ReturnsAsync(new User("username", "passwordHash", "email", Role.User));

            var exception = Assert.ThrowsAsync<UsernameAlreadyExistsException>(async () => await _cut.RegisterUserAsync(request));
            Assert.That(exception.Message, Does.StartWith($"User with username {request.Username} already exists."));
        }
        [Test]
        public async Task RegisterUserAsync_ReturnUserResponse_IfCorrectRequest()
        {
            var request = new RegisterUserRequest("username", "password", "email");
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
            var user = new User("username", passwordHash, "email", Role.User);
            var expected = user.ToResponse();

            var result = await _cut.RegisterUserAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(expected.Username));
            Assert.That(result.Email, Is.EqualTo(expected.Email));
            _userRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(mock => mock.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
        }
        [Test]
        public void LoginUser_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.LoginUserAsync(null));
            Assert.That(exception.Message, Does.StartWith("Register user request can not be null."));
        }
        [Test]
        public async Task LoginUser_ReturnsNull_IfUsernameFoundInDB()
        {
            var request = new LoginUserRequest("username", "password");
            _userRepositoryMock.Setup(mock => mock.GetByUsernameAsync(request.Username)).ReturnsAsync((User)null);

            var result = await _cut.LoginUserAsync(request);

            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task LoginUser_ReturnsNull_IfWrongPasswordProvided()
        {
            var request = new LoginUserRequest("username", "password");
            var user = new User("username", BCrypt.Net.BCrypt.HashPassword("password123"), "email", Role.User);
            _userRepositoryMock.Setup(mock => mock.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

            var result = await _cut.LoginUserAsync(request);

            Assert.That(result, Is.Null);
        }
        [Test]
        public void LoginUser_ThrowsKeyNotFoundException_IfJwtKeyNotFoundInConfig()
        {
            var request = new LoginUserRequest("username", "password");
            var user = new User("username", BCrypt.Net.BCrypt.HashPassword("password"), "email", Role.User);
            _userRepositoryMock.Setup(mock => mock.GetByUsernameAsync(request.Username)).ReturnsAsync(user);
            _configurationMock.Setup(mock => mock["Jwt:Key"]).Returns((string)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.LoginUserAsync(request));
            Assert.That(exception.Message, Does.StartWith("Jwt key not found."));
        }
        [Test]
        public async Task LoginUser_ReturnsJwtToken_IfCorrectRequest()
        {
            var request = new LoginUserRequest("username", "password");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            var user = new User(request.Username, hashedPassword, "email", Role.User);
            _userRepositoryMock.Setup(mock => mock.GetByUsernameAsync(request.Username)).ReturnsAsync(user);
            _configurationMock.Setup(mock => mock["Jwt:Key"]).Returns("jwtKeytiurhgytjfudnckvoejgdfasfsdafhdgs");

            var result = await _cut.LoginUserAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            _userRepositoryMock.Verify(mock => mock.GetByUsernameAsync(request.Username), Times.Once);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(result);
            Assert.That(jsonToken.Issuer, Is.EqualTo("AppIssuer"));
            Assert.That(jsonToken.Audiences.FirstOrDefault(), Is.EqualTo("AppAudience"));
            Assert.That(jsonToken.Claims.Count(), Is.EqualTo(6));
        }
        [Test]
        public void DeleteUserAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.DeleteUserAsync(null));
            Assert.That(exception.Message, Does.StartWith("Delete user request can not be null."));
        }
        [Test]
        public void DeleteUserAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.DeleteUserAsync(
                new DeleteUserRequest(id)));
            Assert.That(exception.Message, Is.EqualTo($"User with id {id} not found."));
        }
        [Test]
        public async Task DeleteUserAsync_DeletesUser_IfCorrectRequest()
        {
            var user = new User("username", "passwordHash", "email", Role.Admin);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            await _cut.DeleteUserAsync(new DeleteUserRequest(user.Id));

            _userRepositoryMock.Verify(mock => mock.DeleteAsync(user.Id), Times.Once);
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
        }
        [Test]
        public void UpdateUserAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.UpdateUserAsync(null));
            Assert.That(exception.Message, Does.StartWith("Update user request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateUserAsync_ThrowsArgumentException_IfUsernameIsEmpty(string username)
        {
            var id = Guid.NewGuid();
            var request = new UpdateUserRequest(id, username, "smth");

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdateUserAsync(request));
            Assert.That(exception.Message, Does.StartWith("Username is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Username)));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateUserAsync_ThrowsArgumentException_IfUserEmailIsEmpty(string email)
        {
            var id = Guid.NewGuid();
            var request = new UpdateUserRequest(id, "smth", email);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdateUserAsync(request));
            Assert.That(exception.Message, Does.StartWith("Email is required"));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Email)));
        }
        [Test]
        public void UpdateUserAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);
            var request = new UpdateUserRequest(Guid.NewGuid(), "username", "email");

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdateUserAsync(request));
            Assert.That(exception.Message, Does.StartWith($"User with id {request.Id} not found."));
        }
        [Test]
        public async Task UpdateUserAsync_UpdatesUser_IfCorrectRequest()
        {
            var user = new User("username", "passwordHash", "email", Role.User);
            var request = new UpdateUserRequest(user.Id, "testUser", "emailUser");
            var expected = new User(request.Username, user.PasswordHash, request.Email, user.Role)
            {
                Id = user.Id
            };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _cut.UpdateUserAsync(request);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.Username, Is.EqualTo(expected.Username));
            Assert.That(result.Email, Is.EqualTo(expected.Email));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
        }
        [Test]
        public async Task GetAllUsersAsync_ReturnsNull_IfNoUsers()
        {
            _userRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync((ICollection<User>)null);

            var result = await _cut.GetAllUsersAsync();

            Assert.That(result, Is.Null);
            _userRepositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }
        [Test]
        public async Task GetAllUsersAsync_ReturnsAllUserDTOs()
        {
            var users = new List<User>()
            {
                new User("username1", "passwordHash1", "email1", Role.User),
                new User("username2", "passwordHash2", "email2", Role.Admin),
                new User("username3", "passwordHash3", "email3", Role.User)
            };
            _userRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(users);
            var expected = new List<UserResponseDTO>()
            {
                users[0].ToResponse(),
                users[1].ToResponse(),
                users[2].ToResponse()
            };

            var result = await _cut.GetAllUsersAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(expected.Count));
            CollectionAssert.AreEqual(result, expected);
        }
        [Test]
        public void GetUserByIdAsync_ThrowsArgumentException_IfIdIsEmpty()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.GetUserById(Guid.Empty));
            Assert.That(exception.Message, Does.StartWith("User id can not be empty."));
        }
        [Test]
        public void GetUserByIdAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.GetUserById(Guid.NewGuid()));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetUserByIdAsync_ReturnsCorrectUserDTO_IfCorrectRequest()
        {
            var user = new User("username", "passwordHash", "email", Role.Admin);
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(user.Id)).ReturnsAsync(user);
            var expected = user.ToResponse();

            var result = await _cut.GetUserById(user.Id);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.Username, Is.EqualTo(expected.Username));
            Assert.That(result.Email, Is.EqualTo(expected.Email));
            Assert.That(result.Role, Is.EqualTo(expected.Role));
        }
    }
}
