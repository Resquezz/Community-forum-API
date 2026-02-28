using CommunityForum.Application.Authorization;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Exceptions;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class TagServiceTests
    {
        private Mock<ITagRepository> _tagRepositoryMock;
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<TagService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _tagRepositoryMock = new Mock<ITagRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<TagService>>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        }

        private static ForumAuthorizationService CreateAuthorizationService(Guid userId, Role role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            };

            return new ForumAuthorizationService(new HttpContextAccessor { HttpContext = context });
        }

        [Test]
        public void CreateTagAsync_ThrowsForbiddenException_ForUserRole()
        {
            var cut = new TagService(
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.User));

            var exception = Assert.ThrowsAsync<ForbiddenException>(() => cut.CreateTagAsync(new CreateTagRequest("dotnet")));

            Assert.That(exception!.Message, Does.StartWith("Only moderator or admin can manage tags."));
        }

        [Test]
        public async Task CreateTagAsync_ReturnsResponse_ForAdminRoleAndValidRequest()
        {
            var request = new CreateTagRequest("dotnet");
            var cut = new TagService(
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.Admin));

            var result = await cut.CreateTagAsync(request);

            Assert.That(result.Name, Is.EqualTo(request.Name));
            _tagRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Tag>()), Times.Once);
        }

        [Test]
        public void UpdateTagAsync_ThrowsKeyNotFoundException_IfTagNotFound()
        {
            var request = new UpdateTagRequest(Guid.NewGuid(), "new-tag");
            _tagRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id)).ReturnsAsync((Tag?)null);

            var cut = new TagService(
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.Moderator));

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => cut.UpdateTagAsync(request));

            Assert.That(exception!.Message, Does.StartWith($"Tag with id {request.Id} not found."));
        }
    }
}
