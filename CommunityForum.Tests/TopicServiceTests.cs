using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Services;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework.Legacy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityForum.Application.Mappers;
using Azure.Core;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class TopicServiceTests
    {
        private Mock<IHubContext<ForumHub>> _hubContextMock;
        private Mock<ILogger<TopicService>> _loggerMock;
        private Mock<ITopicRepository> _topicRepositoryMock;
        private TopicService _cut;
        [SetUp]
        public void SetUp()
        {
            _topicRepositoryMock = new Mock<ITopicRepository>();
            _hubContextMock = new Mock<IHubContext<ForumHub>>();
            _loggerMock = new Mock<ILogger<TopicService>>();
            _hubContextMock.Setup(hub => hub.Clients.All
                .SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _cut = new TopicService(_topicRepositoryMock.Object, _hubContextMock.Object, _loggerMock.Object);
        }
        [Test]
        public void CreateTopicAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.CreateTopicAsync(null));
            Assert.That(exception.Message, Does.StartWith("Create topic request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CreateTopicAsync_ThrowsArgumentException_IfTitleIsEmpty(string title)
        {
            var request = new CreateTopicRequest(title, "smth");

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.CreateTopicAsync(request));
            Assert.That(exception.Message, Does.StartWith("Topic title is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Title)));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void CreateTopicAsync_ThrowsArgumentException_IfDescriptionIsEmpty(string description)
        {
            var request = new CreateTopicRequest("smth", description);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.CreateTopicAsync(request));
            Assert.That(exception.Message, Does.StartWith("Topic description is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Description)));
        }
        [Test]
        public async Task CreateTopicAsync_ReturnTopicResponse_IfCorrectRequest()
        {
            var request = new CreateTopicRequest("title", "description");
            var expected = new TopicResponseDTO(Guid.NewGuid(), "title", "description");

            var result = await _cut.CreateTopicAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(expected.Title));
            Assert.That(result.Description, Is.EqualTo(expected.Description));
            _topicRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Topic>()), Times.Once);
        }
        [Test]
        public void DeleteTopicAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.DeleteTopicAsync(null));
            Assert.That(exception.Message, Does.StartWith("Delete topic request can not be null."));
        }
        [Test]
        public void DeleteTopicAsync_ThrowsKeyNotFoundException_IfTopicNotFoundInDB()
        {
            var id = Guid.NewGuid();
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.DeleteTopicAsync(
                new DeleteTopicRequest(id)));
            Assert.That(exception.Message, Is.EqualTo($"Topic with id {id} not found."));
        }
        [Test]
        public async Task DeletePostAsync_DeletesComment_IfCorrectRequest()
        {
            var topic = new Topic("title", "description");
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(topic.Id)).ReturnsAsync(topic);

            await _cut.DeleteTopicAsync(new DeleteTopicRequest(topic.Id));

            _topicRepositoryMock.Verify(mock => mock.DeleteAsync(topic.Id), Times.Once);
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(topic.Id), Times.Once);
        }
        [Test]
        public void UpdateTopicAsync_ThrowsArgumentNullException_IfRequestIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _cut.UpdateTopicAsync(null));
            Assert.That(exception.Message, Does.StartWith("Update topic request can not be null."));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateTopicAsync_ThrowsArgumentException_IfTopicTitleIsEmpty(string title)
        {
            var id = Guid.NewGuid();
            var request = new UpdateTopicRequest(id, title, "smth");

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdateTopicAsync(request));
            Assert.That(exception.Message, Does.StartWith("Topic title is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Title)));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateTopicAsync_ThrowsArgumentException_IfTopicDescriptionIsEmpty(string description)
        {
            var id = Guid.NewGuid();
            var request = new UpdateTopicRequest(id, "smth", description);

            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.UpdateTopicAsync(request));
            Assert.That(exception.Message, Does.StartWith("Topic description is required."));
            Assert.That(exception.ParamName, Is.EqualTo(nameof(request.Description)));
        }
        [Test]
        public void UpdateTopicAsync_ThrowsKeyNotFoundException_IfTopicNotFoundInDB()
        {
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic)null);
            var request = new UpdateTopicRequest(Guid.NewGuid(), "title", "description");

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.UpdateTopicAsync(request));
            Assert.That(exception.Message, Does.StartWith($"Topic with id {request.Id} not found."));
        }
        [Test]
        public async Task UpdateCommentAsync_UpdatesComment_IfCorrectRequest()
        {
            var topic = new Topic("smth1", "smth2");
            var request = new UpdateTopicRequest(topic.Id, "title", "description");
            var expected = new Topic(request.Title, request.Description)
            {
                Id = topic.Id
            };
            _topicRepositoryMock.Setup(repo => repo.GetByIdAsync(topic.Id)).ReturnsAsync(topic);

            var result = await _cut.UpdateTopicAsync(request);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.Title, Is.EqualTo(expected.Title));
            Assert.That(result.Description, Is.EqualTo(expected.Description));
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(topic.Id), Times.Once);
        }
        [Test]
        public async Task GetAllTopicsAsync_ReturnsNull_IfNoTopics()
        {
            _topicRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync((ICollection<Topic>)null);

            var result = await _cut.GetAllTopicsAsync();

            Assert.That(result, Is.Null);
            _topicRepositoryMock.Verify(mock => mock.GetAllAsync(), Times.Once);
        }
        [Test]
        public async Task GetAllTopicsAsync_ReturnsAllTopicDTOs()
        {
            var topics = new List<Topic>()
            {
                new Topic("title1", "desc1"),
                new Topic("title2", "desc2"),
                new Topic("title3", "desc3")
            };
            _topicRepositoryMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(topics);
            var expected = new List<TopicResponseDTO>()
            {
                topics[0].ToResponse(),
                topics[1].ToResponse(),
                topics[2].ToResponse()
            };

            var result = await _cut.GetAllTopicsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(expected.Count));
            CollectionAssert.AreEqual(result, expected);
        }
        [Test]
        public void GetTopicByIdAsync_ThrowsArgumentException_IfIdIsEmpty()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _cut.GetTopicByIdAsync(Guid.Empty));
        }
        [Test]
        public void GetTopicByIdAsync_ThrowsKeyNotFoundException_IfTopicNotFoundInDB()
        {
            _topicRepositoryMock.Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Topic)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _cut.GetTopicByIdAsync(Guid.NewGuid()));
            _topicRepositoryMock.Verify(mock => mock.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
        [Test]
        public async Task GetTopicByIdAsync_ReturnsCorrectTopicDTO_IfCorrectRequest()
        {
            var topic = new Topic("title", "desc");
            _topicRepositoryMock.Setup(mock => mock.GetByIdAsync(topic.Id)).ReturnsAsync(topic);
            var expected = topic.ToResponse();

            var result = await _cut.GetTopicByIdAsync(topic.Id);

            Assert.That(result.Id, Is.EqualTo(expected.Id));
            Assert.That(result.Title, Is.EqualTo(expected.Title));
            Assert.That(result.Description, Is.EqualTo(expected.Description));
        }
    }
}
