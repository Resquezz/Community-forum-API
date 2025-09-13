﻿using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ILogger<TopicService> _logger;
        public TopicService(ITopicRepository topicRepository, IHubContext<ForumHub> hubContext, ILogger<TopicService> logger)
        {
            _topicRepository = topicRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<TopicResponseDTO> CreateTopicAsync(CreateTopicRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to create topic with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create topic request can not be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogError("Attempt to create topic without title.");
                throw new ArgumentException("Topic title is required.", nameof(request.Title));
            }   
            if(string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogError("Attempt to create topic without description.");
                throw new ArgumentException("Topic description is required.", nameof(request.Description));
            }

            var topic = new Topic(request.Title, request.Description);
            await _topicRepository.AddAsync(topic);
            var response = topic.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.TopicCreated.ToString(), response);
            _logger.LogInformation("Topic created successfully.");
            return response;
        }

        public async Task DeleteTopicAsync(DeleteTopicRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete topic with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete topic request can not be null.");
            }
            var topic = await _topicRepository.GetByIdAsync(request.Id);
            if (topic == null)
            {
                _logger.LogError("Attempt to delete non existing topic. Topic id: {topicId}", request.Id);
                throw new KeyNotFoundException($"Topic with id {request.Id} not found.");
            }

            await _topicRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.TopicDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Topic deleted successfully.");
        }

        public async Task<TopicResponseDTO> UpdateTopicAsync(UpdateTopicRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update topic with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update topic request can not be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogError("Attempt to update topic title with empty. Topic id: {topicId}", request.Id);
                throw new ArgumentException("Topic title is required.", nameof(request.Title));
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogError("Attempt to update topic description with empty. Topic id: {topicId}", request.Id);
                throw new ArgumentException("Topic description is required.", nameof(request.Description));
            }

            var topic = await _topicRepository.GetByIdAsync(request.Id);
            if (topic == null)
            {
                _logger.LogError("Attempt to update non existing topic. Topic id: {topicId}", request.Id);
                throw new KeyNotFoundException($"Topic with id {request.Id} not found.");
            }

            topic.Title = request.Title;
            topic.Description = request.Description;

            await _topicRepository.UpdateAsync(topic);
            var response = topic.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.TopicUpdated.ToString(), response);
            _logger.LogInformation("Topic updated successfully.");
            return response;
        }
        public async Task<ICollection<TopicResponseDTO>?> GetAllTopicsAsync()
        {
            var topics = await _topicRepository.GetAllAsync();
            if(topics == null)
            {
                _logger.LogInformation("No topics to fetch from database.");
                return null;
            }
            _logger.LogInformation("Successfully fetched {count} topics from database.", topics.Count);
            return topics.Select(topic => topic.ToResponse()).ToList();
        }
        public async Task<TopicResponseDTO> GetTopicByIdAsync(Guid id)
        {
            if (Guid.Empty == id)
            {
                _logger.LogError("Atempt to retrieve topic with empty id.");
                throw new ArgumentException(nameof(id), "Topic id can not be empty.");
            }
            var topic = await _topicRepository.GetByIdAsync(id);
            if (topic == null)
            {
                _logger.LogError("Atempt to retrieve non existing topic. Topic id: {topicId}", id);
                throw new KeyNotFoundException($"Topic with id {id} not found.");
            }
            _logger.LogInformation("Topic with id {id} retrieved successfully.", id);
            return topic.ToResponse();
        }
    }
}
