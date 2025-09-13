using Castle.Core.Logging;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System.ComponentModel.DataAnnotations;
using Azure.Core;
using System.Reflection.Metadata;
using CommunityForum.Application.Mappers;
using NUnit.Framework.Legacy;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICommentRepository> _commentRepositoryMock;
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<CommentService>> _loggerMock;
        private CommentService _cut;
        [SetUp]
        public void SetUp()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<CommentService>>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cut = new CommentService(_commentRepositoryMock.Object, _postRepositoryMock.Object, _userRepositoryMock.Object,
                _hubContextMock.Object, _loggerMock.Object);
        }
        [Test]
        public void CreateCommentAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateCommentAsync(null));
            Assert.That(exception.Message, Does.StartWith("Create comment request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CreateCommentAsync_ThrowsArgumentException_IfContentIsEmpty(string content)
        {
            var request = new CreateCommentRequest(content, Guid.NewGuid(), Guid.NewGuid());

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Comment content is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Content)));
        }
        [Test]
        public void CreateCommentAsync_ThrowsArgumentNullException_IfUserIdIsEmpty()
        {
            var request = new CreateCommentRequest("smth", Guid.Empty, Guid.NewGuid());

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("User is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.UserId)));
        }
        [Test]
        public void CreateCommentAsync_ThrowsArgumentNullException_IfPostIdIsEmpty()
        {
            var request = new CreateCommentRequest("smth", Guid.NewGuid(), Guid.Empty);

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Post is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.PostId)));
        }
        [Test]
        public void CreateCommentAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            var request = new CreateCommentRequest("smth", Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Can not find user or post."));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
        }
        [Test]
        public void CreateCommentAsync_ThrowsKeyNotFoundException_IfPostNotFoundInDB()
        {
            var request = new CreateCommentRequest("smth", Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User("user", "123", "email", Role.User));
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Can not find user or post."));
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public void CreateCommentAsync_ThrowsKeyNotFoundException_IfParentCommentNotFoundInDB()
        {
            var request = new CreateCommentRequest("smth", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User("user", "123", "email", Role.User));
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Post("smth", Guid.NewGuid(), Guid.NewGuid()));
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Can not find parent comment."));
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task CreateCommentAsync_ReturnCommentResponse_IfCorrectRequest()
        {
            var user = new User("smth", "123", "email", Role.User);
            var post = new Post("smth", user.Id, Guid.NewGuid());
            var parentComment = new Comment("smth", user.Id, post.Id);
            var request = new CreateCommentRequest("smth", user.Id, post.Id, parentComment.Id);
            var newComment = new Comment("smth", request.UserId, request.PostId, request.ParentCommentId);
            var expected = new CommentResponseDTO(newComment.Id, newComment.Content, newComment.UserId,
                newComment.PostId, newComment.ParentCommentId);

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(parentComment);

            var result = await _cut.CreateCommentAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.ParentCommentId, Is.EqualTo(expected.ParentCommentId));
            Assert.That(result.PostId, Is.EqualTo(expected.PostId));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(post.Id), Times.Once);
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(parentComment.Id), Times.Once);
        }
        [Test]
        public void DeleteCommentAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.DeleteCommentAsync(null));
            Assert.That(exception.Message, Does.StartWith("Delete comment request can not be null."));
        }
        [Test]
        public void DeleteCommentAsync_ThrowsKeyNotFoundException_IfCommentNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.DeleteCommentAsync(
                new DeleteCommentRequest(id)));
            Assert.That(exception.Message, Is.EqualTo($"Comment with id {id} not found."));
        }
        [Test]
        public async Task DeleteCommentAsync_DeletesComment_IfCorrectRequest()
        {
            var comment = new Comment("smth", Guid.NewGuid(), Guid.NewGuid());
            _commentRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(comment);

            await _cut.DeleteCommentAsync(new DeleteCommentRequest(comment.Id));

            _commentRepositoryMock.Verify(mock => mock.DeleteAsync(comment.Id), Times.Once);
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(comment.Id), Times.Once);
        }
        [Test]
        public void UpdateCommentAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.UpdateCommentAsync(null));
            Assert.That(exception.Message, Does.StartWith("Update comment request can not be null."));
        }
        [Test]
        public void UpdateCommentAsync_ThrowsKeyNotFoundException_IfCommentNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdateCommentAsync(
                new UpdateCommentRequest(id, "smth")));
            Assert.That(exception.Message, Is.EqualTo($"Comment with id {id} not found."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateCommentAsync_ThrowsArgumentException_IfCommentContentIsEmpty(string content)
        {
            var id = Guid.NewGuid();
            var request = new UpdateCommentRequest(id, content);
            var comment = new Comment("smth", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            _commentRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(comment);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdateCommentAsync(request));
            Assert.That(exception.Message, Does.StartWith("Comment content is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Content)));
        }
        [Test]
        public async Task UpdateCommentAsync_UpdatesComment_IfCorrectRequest()
        {
            var comment = new Comment("smth", Guid.NewGuid(), Guid.NewGuid());
            var request = new UpdateCommentRequest(comment.Id, "123");
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(comment.Id)).ReturnsAsync(comment);

            var result = await _cut.UpdateCommentAsync(request);

            Assert.That(result.Id, Is.EqualTo(comment.Id));
            Assert.That(result.ParentCommentId, Is.EqualTo(comment.ParentCommentId));
            Assert.That(result.PostId, Is.EqualTo(comment.PostId));
            Assert.That(result.UserId, Is.EqualTo(comment.UserId));
            Assert.That(result.Content, Is.EqualTo(request.Content));
            _commentRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<Comment>()), Times.Once);
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(comment.Id), Times.Once);
        }
        [Test]
        public async Task GetAllCommentsAsync_ReturnsNull_IfNoComments()
        {
            _commentRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync((ICollection<Comment>)null);

            var result = await _cut.GetAllCommentsAsync();

            Assert.That(result, Is.Null);
            _commentRepositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }
        [Test]
        public async Task GetAllCommentsAsync_ReturnsAllCommentDTOs()
        {
            var comments = new List<Comment>
            {
                new Comment("smth", Guid.NewGuid(), Guid.NewGuid()),
                new Comment("smth1", Guid.NewGuid(), Guid.NewGuid()),
                new Comment("smth2", Guid.NewGuid(), Guid.NewGuid())
            };
            ICollection<CommentResponseDTO> expected = new List<CommentResponseDTO>
            {
                comments[0].ToResponse(),
                comments[1].ToResponse(),
                comments[2].ToResponse()
            };
            _commentRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(comments);

            var result = await _cut.GetAllCommentsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(comments.Count));
            CollectionAssert.AreEqual(result, expected);
        }
        [Test]
        public void GetCommentByIdAsync_ThrowsKeyNotFoundException_IfCommentNotFoundInDB()
        {
            _commentRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.GetCommentByIdAsync(Guid.NewGuid()));
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetCommentByIdAsync_ReturnsCorrectCommentDTO_IfCorrectRequest()
        {
            var comment = new Comment("smth", Guid.NewGuid(), Guid.NewGuid());
            _commentRepositoryMock.Setup(mock => mock.GetByIdAsync(comment.Id)).ReturnsAsync(comment);

            var result = await _cut.GetCommentByIdAsync(comment.Id);

            Assert.That(result.Id, Is.EqualTo(comment.Id));
            Assert.That(result.UserId, Is.EqualTo(comment.UserId));
            Assert.That(result.Content, Is.EqualTo(comment.Content));
        }
    }
}
