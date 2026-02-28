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
    public class PostTagServiceTests
    {
        private Mock<IPostTagRepository> _postTagRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<ITagRepository> _tagRepositoryMock;
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<PostTagService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _postTagRepositoryMock = new Mock<IPostTagRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<PostTagService>>();
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
        public void AddPostTagAsync_ThrowsForbiddenException_ForNotOwnerUser()
        {
            var currentUserId = Guid.NewGuid();
            var postOwnerId = Guid.NewGuid();
            var request = new CreatePostTagRequest(Guid.NewGuid(), Guid.NewGuid());
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(request.PostId))
                .ReturnsAsync(new Post("content", postOwnerId, Guid.NewGuid()));

            var cut = new PostTagService(
                _postTagRepositoryMock.Object,
                _postRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(currentUserId, Role.User));

            var exception = Assert.ThrowsAsync<ForbiddenException>(() => cut.AddPostTagAsync(request));

            Assert.That(exception!.Message, Does.StartWith("You can modify only your own post tags."));
        }

        [Test]
        public async Task AddPostTagAsync_ReturnsResponse_ForOwnerAndValidRequest()
        {
            var currentUserId = Guid.NewGuid();
            var request = new CreatePostTagRequest(Guid.NewGuid(), Guid.NewGuid());
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(request.PostId))
                .ReturnsAsync(new Post("content", currentUserId, Guid.NewGuid()));
            _tagRepositoryMock.Setup(repo => repo.GetByIdAsync(request.TagId))
                .ReturnsAsync(new Tag("backend") { Id = request.TagId });
            _postTagRepositoryMock.Setup(repo => repo.GetByIdsAsync(request.PostId, request.TagId))
                .ReturnsAsync((PostTag?)null);

            var cut = new PostTagService(
                _postTagRepositoryMock.Object,
                _postRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(currentUserId, Role.User));

            var result = await cut.AddPostTagAsync(request);

            Assert.That(result.PostId, Is.EqualTo(request.PostId));
            Assert.That(result.TagId, Is.EqualTo(request.TagId));
            _postTagRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<PostTag>()), Times.Once);
        }

        [Test]
        public async Task GetTagsByPostIdAsync_ReturnsTags_ForExistingPost()
        {
            var postId = Guid.NewGuid();
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(new Post("content", Guid.NewGuid(), Guid.NewGuid()));
            _postTagRepositoryMock.Setup(repo => repo.GetByPostIdAsync(postId))
                .ReturnsAsync(new List<PostTag>
                {
                    new PostTag(postId, Guid.NewGuid()) { Tag = new Tag("dotnet") },
                    new PostTag(postId, Guid.NewGuid()) { Tag = new Tag("api") }
                });

            var cut = new PostTagService(
                _postTagRepositoryMock.Object,
                _postRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                CreateAuthorizationService(Guid.NewGuid(), Role.Admin));

            var result = await cut.GetTagsByPostIdAsync(postId);

            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
