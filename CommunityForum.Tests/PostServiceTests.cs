using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class PostServiceTests
    {
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<PostService>> _loggerMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ITopicRepository> _topicRepositoryMock;
        private PostService _cut;
        [SetUp]
        public void SetUp()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _topicRepositoryMock = new Mock<ITopicRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<PostService>>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cut = new PostService(_postRepositoryMock.Object, _userRepositoryMock.Object, _topicRepositoryMock.Object,
                _hubContextMock.Object, _loggerMock.Object);
        }
        [Test]
        public void CreatePostAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreatePostAsync(null));
            Assert.That(exception.Message, Does.StartWith("Create post request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CreatePostAsync_ThrowsArgumentException_IfContentIsEmpty(string content)
        {
            var request = new CreatePostRequest(content, Guid.NewGuid(), Guid.NewGuid());

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.CreatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("Post content is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Content)));
        }
        [Test]
        public void CreatePostAsync_ThrowsArgumentNullException_IfUserIdIsEmpty()
        {
            var request = new CreatePostRequest("smth", Guid.Empty, Guid.NewGuid());

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("User is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.UserId)));
        }
        [Test]
        public void CreatePostAsync_ThrowsArgumentNullException_IfTopicIdIsEmpty()
        {
            var request = new CreatePostRequest("smth", Guid.NewGuid(), Guid.Empty);

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("Topic is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.TopicId)));
        }
        [Test]
        public void CreatePostAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            var request = new CreatePostRequest("smth", Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("Can not find user or topic."));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
        }
        [Test]
        public void CreatePostAsync_ThrowsKeyNotFoundException_IfTopicNotFoundInDB()
        {
            var request = new CreatePostRequest("smth", Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User("user", "123", "email", Role.User));
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.CreatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("Can not find user or topic."));
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task CreatePostAsync_ReturnPostResponse_IfCorrectRequest()
        {
            var user = new User("smth", "123", "email", Role.User);
            var topic = new Topic("smth1", "smth2");
            var post = new Post("smthContent", user.Id, topic.Id);
            var expected = new PostResponseDTO(post.Id, post.Content, post.UserId, user.Username, post.TopicId, topic.Title);
            var request = new CreatePostRequest("smthContent", user.Id, topic.Id);
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _topicRepositoryMock.Setup(mock => mock.GetByIdAsync(topic.Id)).ReturnsAsync(topic);

            var result = await _cut.CreatePostAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.EqualTo(expected.Content));
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.TopicId, Is.EqualTo(expected.TopicId));
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(user.Id), Times.Once);
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(topic.Id), Times.Once);
        }
        [Test]
        public void DeletePostAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.DeletePostAsync(null));
            Assert.That(exception.Message, Does.StartWith("Delete post request can not be null."));
        }
        [Test]
        public void DeletePostAsync_ThrowsKeyNotFoundException_IfPostNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.DeletePostAsync(
                new DeletePostRequest(id)));
            Assert.That(exception.Message, Is.EqualTo($"Post with id {id} not found."));
        }
        [Test]
        public async Task DeletePostAsync_DeletesComment_IfCorrectRequest()
        {
            var post = new Post("smth", Guid.NewGuid(), Guid.NewGuid());
            _postRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);

            await _cut.DeletePostAsync(new DeletePostRequest(post.Id));

            _postRepositoryMock.Verify(mock => mock.DeleteAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(post.Id), Times.Once);
        }
        [Test]
        public void UpdatePostAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.UpdatePostAsync(null));
            Assert.That(exception.Message, Does.StartWith("Update post request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateCommentAsync_ThrowsArgumentException_IfCommentContentIsEmpty(string content)
        {
            var id = Guid.NewGuid();
            var request = new UpdatePostRequest(id, content);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("Post content is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Content)));
        }
        [Test]
        public void UpdatePostAsync_ThrowsKeyNotFoundException_IfPostNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdatePostAsync(
                new UpdatePostRequest(id, "smth")));
            Assert.That(exception.Message, Is.EqualTo($"Post with id {id} not found."));
        }
        [Test]
        public void UpdatePostAsync_ThrowsKeyNotFoundException_IfUserNotFoundInDB()
        {
            var post = new Post("smth", Guid.NewGuid(), Guid.NewGuid());
            var topic = new Topic("title", "desc");
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(topic);
            var request = new UpdatePostRequest(post.Id, "smth2");

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("User or topic does not exist."));
        }
        [Test]
        public void UpdatePostAsync_ThrowsKeyNotFoundException_IfTopicNotFoundInDB()
        {
            var post = new Post("smth", Guid.NewGuid(), Guid.NewGuid());
            var user = new User("user", "hash", "email", Role.User);
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic)null);
            var request = new UpdatePostRequest(post.Id, "smth2");

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdatePostAsync(request));
            Assert.That(exception.Message, Does.StartWith("User or topic does not exist."));
        }
        [Test]
        public async Task UpdatePostAsync_UpdatesPost_IfCorrectRequest()
        {
            var post = new Post("smth", Guid.NewGuid(), Guid.NewGuid());
            var user = new User("user", "hash", "email", Role.User);
            var topic = new Topic("title", "desc");
            _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(topic);
            var request = new UpdatePostRequest(post.Id, "smth2");
            var expected = new PostResponseDTO(post.Id, "smth2", user.Id, user.Username, topic.Id, topic.Title);

            var result = await _cut.UpdatePostAsync(request);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.Content, Is.EqualTo(expected.Content));
            Assert.That(result.Username, Is.EqualTo(expected.Username));
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.TopicId, Is.EqualTo(expected.TopicId));
            Assert.That(result.TopicTitle, Is.EqualTo(expected.TopicTitle));
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _postRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<Post>()), Times.Once);
            _userRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetAllPostsAsync_ReturnsNull_IfNoPosts()
        {
            _postRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync((ICollection<Post>)null);

            var result = await _cut.GetAllPostsAsync();

            Assert.That(result, Is.Null);
            _postRepositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }
        [Test]
        public async Task GetAllPostsAsync_ReturnsAllPostDTOs()
        {
            var users = new List<User>()
            {
                new User("user1", "hash1", "email1", Role.Admin),
                new User("user2", "hash2", "email2", Role.User),
                new User("user3", "hash3", "email3", Role.Admin)
            };
            var topics = new List<Topic>()
            {
                new Topic("title1", "desc1"),
                new Topic("title2", "desc2"),
                new Topic("title3", "desc3")
            };
            var posts = new List<Post>()
            {
                new Post("smth", users[0].Id, topics[0].Id),
                new Post("smth1", users[1].Id, topics[1].Id),
                new Post("smth2", users[2].Id, topics[2].Id)
            };
            _userRepositoryMock.SetupSequence(mock => mock.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(users[0]).ReturnsAsync(users[1]).ReturnsAsync(users[2]);
            _topicRepositoryMock.SetupSequence(mock => mock.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(topics[0]).ReturnsAsync(topics[1]).ReturnsAsync(topics[2]);
            _postRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(posts);
            var expected = new List<PostResponseDTO>()
            {
                posts[0].ToResponse(users[0], topics[0]),
                posts[1].ToResponse(users[1], topics[1]),
                posts[2].ToResponse(users[2], topics[2])
            };

            var result = await _cut.GetAllPostsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(expected.Count));
            CollectionAssert.AreEqual(result, expected);
        }
        [Test]
        public void GetPostByIdAsync_ThrowsArgumentException_IfIdIsEmpty()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.GetPostByIdAsync(Guid.Empty));
        }
        [Test]
        public void GetPostByIdAsync_ThrowsKeyNotFoundException_IfPostNotFoundInDB()
        {
            _postRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.GetPostByIdAsync(Guid.NewGuid()));
            _postRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetPostByIdAsync_ReturnsCorrectPostDTO_IfCorrectRequest()
        {
            var user = new User("user", "hash", "email", Role.Admin);
            var topic = new Topic("title", "desc");
            var post = new Post("smth", user.Id, topic.Id);
            _postRepositoryMock.Setup(mock => mock.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _userRepositoryMock.Setup(mock => mock.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _topicRepositoryMock.Setup(mock => mock.GetByIdAsync(topic.Id)).ReturnsAsync(topic);
            var expected = post.ToResponse(user, topic);

            var result = await _cut.GetPostByIdAsync(post.Id);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.UserId, Is.EqualTo(expected.UserId));
            Assert.That(result.Content, Is.EqualTo(expected.Content));
        }
    }
}
