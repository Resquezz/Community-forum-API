using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Exceptions;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class VoteServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IVoteRepository> _voteRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<ICommentRepository> _commentRepositoryMock;
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<VoteService>> _loggerMock;
        private VoteService _cut;
        [SetUp]
        public void SetUp()
        {
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<VoteService>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _voteRepositoryMock = new Mock<IVoteRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cut = new VoteService(_userRepositoryMock.Object, _voteRepositoryMock.Object, _postRepositoryMock.Object,
                _commentRepositoryMock.Object, _hubContextMock.Object, _loggerMock.Object);
        }
        [Test]
        public void CreateVoteAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateVoteAsync(null));
            Assert.That(exception.Message, Does.StartWith("Create vote request can not be null."));
        }
        [Test]
        public void CreateVoteAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            var request = new CreateVoteRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), VoteType.DownVote);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateVoteAsync(request));
            Assert.That(exception.Message, Does.StartWith($"User with id {request.UserId} not found."));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
        }
        [Test]
        public void CreateVoteAsync_ThrowsArgumentNullException_IfPostIdAndCommentIdIsEmpty()
        {
            var request = new CreateVoteRequest(Guid.NewGuid(), null, null, VoteType.DownVote);
            var user = new User("username", "passwordHash", "email", Role.User);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateVoteAsync(request));
            Assert.That(exception.Message, Does.StartWith("Vote must be for a post or a comment."));
        }
        [Test]
        public void CreateVoteAsync_ThrowsKeyNotFoundException_IfPostNotFoundIdDb()
        {
            var user = new User("username", "passwordHash", "email", Role.Admin);
            var request = new CreateVoteRequest(user.Id, Guid.NewGuid(), null, VoteType.UpVote);
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _postRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateVoteAsync(request));
            Assert.That(exception.Message, Does.StartWith($"Post with id {request.PostId} not found."));
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public void CreateVoteAsync_ThrowsKeyNotFoundException_IfCommentNotFoundInDB()
        {
            var user = new User("username", "passwordHash", "email", Role.User);
            var request = new CreateVoteRequest(user.Id, null, Guid.NewGuid(), VoteType.UpVote);
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _commentRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreateVoteAsync(request));
            Assert.That(exception.Message, Does.StartWith($"Comment with id {request.CommentId} not found."));
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task CreateVoteAsync_ReturnVoteResponse_IfCorrectPostVoteRequest()
        {
            var user = new User("username", "passwordHash", "email", Role.User);
            var post = new Post("content", user.Id, Guid.NewGuid());
            var request = new CreateVoteRequest(user.Id, post.Id, null, VoteType.UpVote);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            var vote = new Vote(request.UserId, request.VoteType, request.PostId);
            var expected = vote.ToResponse();

            var result = await _cut.CreateVoteAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.PostId, Is.EqualTo(expected.PostId));
            Assert.That(result.VoteType, Is.EqualTo(expected.VoteType));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(post.Id), Times.Once);
        }
        [Test]
        public async Task CreateVoteAsync_ReturnVoteResponse_IfCorrectCommentVoteRequest()
        {
            var user = new User("username", "passwordHash", "email", Role.User);
            var comment = new Comment("content", user.Id, Guid.NewGuid(), Guid.NewGuid());
            var request = new CreateVoteRequest(user.Id, null, comment.Id, VoteType.UpVote);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(comment);
            var vote = new Vote(request.UserId, request.VoteType, null, request.CommentId);
            var expected = vote.ToResponse();

            var result = await _cut.CreateVoteAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.PostId, Is.EqualTo(expected.PostId));
            Assert.That(result.VoteType, Is.EqualTo(expected.VoteType));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
            _commentRepositoryMock.Verify(mock => mock.GetByIdAsync(comment.Id), Times.Once);
        }
        [Test]
        public void DeleteVoteAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.DeleteVoteAsync(null));
            Assert.That(exception.Message, Does.StartWith("Delete vote request can not be null."));
        }
        [Test]
        public void DeleteVoteAsync_ThrowsKeyNotFoundException_IfVoteNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _voteRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync((Vote)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.DeleteVoteAsync(
                new DeleteVoteRequest(id)));
            Assert.That(exception.Message, Does.StartWith($"Vote with id {id} not found."));
        }
        [Test]
        public async Task DeleteVoteAsync_DeletesVote_IfCorrectRequest()
        {
            var vote = new Vote(Guid.NewGuid(), VoteType.DownVote, Guid.NewGuid(), null);
            _voteRepositoryMock.Setup(mock => mock.GetByIdAsync(vote.Id)).ReturnsAsync(vote);

            await _cut.DeleteVoteAsync(new DeleteVoteRequest(vote.Id));

            _voteRepositoryMock.Verify(mock => mock.DeleteAsync(vote.Id), Times.Once);
            _voteRepositoryMock.Verify(mock => mock.GetByIdAsync(vote.Id), Times.Once);
        }
        [Test]
        public void UpdateVoteAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.UpdateVoteAsync(null));
            Assert.That(exception.Message, Does.StartWith("Update vote request can not be null."));
        }
        [Test]
        public void UpdateVoteAsync_ThrowsKeyNotFoundException_IfVoteNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _voteRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vote)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdateVoteAsync(
                new UpdateVoteRequest(id, VoteType.UpVote)));
            Assert.That(exception.Message, Does.StartWith($"Vote with id {id} not found."));
        }
        [Test]
        public async Task UpdateVoteAsync_UpdatesVote_IfCorrectRequest()
        {
            var vote = new Vote(Guid.NewGuid(), VoteType.UpVote, Guid.NewGuid());
            var request = new UpdateVoteRequest(vote.Id, VoteType.DownVote);
            var expected = new VoteResponseDTO(vote.Id, vote.UserId, vote.PostId, vote.CommentId, request.VoteType);
            _voteRepositoryMock.Setup(repo => repo.GetByIdAsync(vote.Id)).ReturnsAsync(vote);

            var result = await _cut.UpdateVoteAsync(request);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.CommentId, Is.EqualTo(expected.CommentId));
            Assert.That(result.PostId, Is.EqualTo(expected.PostId));
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.VoteType, Is.EqualTo(expected.VoteType));
            _voteRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<Vote>()), Times.Once);
            _voteRepositoryMock.Verify(mock => mock.GetByIdAsync(expected.Id), Times.Once);
        }
        [Test]
        public void GetVoteByIdAsync_ThrowsArgumentNullException_IfIdIsEmpty()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.GetVoteByIdAsync(Guid.Empty));
            Assert.That(exception.Message, Does.StartWith("Vote id can not be empty."));
        }
        [Test]
        public void GetVoteByIdAsync_ThrowsKeyNotFoundException_IfVoteNotFoundInDB()
        {
            _voteRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vote)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.GetVoteByIdAsync(Guid.NewGuid()));
            _voteRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetVoteByIdAsync_ReturnsCorrectVoteDTO_IfCorrectRequest()
        {
            var vote = new Vote(Guid.NewGuid(), VoteType.DownVote, null, Guid.NewGuid());
            _voteRepositoryMock.Setup(mock => mock.GetByIdAsync(vote.Id)).ReturnsAsync(vote);

            var result = await _cut.GetVoteByIdAsync(vote.Id);

            Assert.That(result.Id, Is.EqualTo(vote.Id));
            Assert.That(result.UserId, Is.EqualTo(vote.UserId));
            Assert.That(result.CommentId, Is.EqualTo(vote.CommentId));
            Assert.That(result.PostId, Is.EqualTo(vote.PostId));
            Assert.That(result.VoteType, Is.EqualTo(vote.VoteType));
        }
    }
}
