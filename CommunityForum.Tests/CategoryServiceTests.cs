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
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<CategoryService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
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
        public void CreateCategoryAsync_ThrowsForbiddenException_ForUserRole()
        {
            var cut = new CategoryService(
                _categoryRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.User));

            var exception = Assert.ThrowsAsync<ForbiddenException>(() => cut.CreateCategoryAsync(new CreateCategoryRequest("name", "desc")));

            Assert.That(exception!.Message, Does.StartWith("Only moderator or admin can manage categories."));
        }

        [Test]
        public async Task CreateCategoryAsync_ReturnsResponse_ForAdminRoleAndValidRequest()
        {
            var request = new CreateCategoryRequest("General", "General category");
            var cut = new CategoryService(
                _categoryRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.Admin));

            var result = await cut.CreateCategoryAsync(request);

            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.Description, Is.EqualTo(request.Description));
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
        }

        [Test]
        public void CreateCategoryAsync_ThrowsInvalidOperationException_IfCategoryAlreadyExists()
        {
            var request = new CreateCategoryRequest("General", "General category");
            _categoryRepositoryMock.Setup(repo => repo.GetByNameAsync("General"))
                .ReturnsAsync(new Category("General", "existing"));

            var cut = new CategoryService(
                _categoryRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.Admin));

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => cut.CreateCategoryAsync(request));

            Assert.That(exception!.Message, Does.StartWith("Category 'General' already exists."));
        }
    }
}
