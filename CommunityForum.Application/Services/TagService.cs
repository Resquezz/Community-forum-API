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
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagRepository tagRepository, IHubContext<ForumHub> hubContext, ILogger<TagService> logger)
        {
            _tagRepository = tagRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<TagResponseDTO> CreateTagAsync(CreateTagRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to create tag with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create tag request can not be null.");
            }

            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                _logger.LogError("Attempt to create tag without name.");
                throw new ArgumentException("Tag name is required.", nameof(request.Name));
            }

            var existingTag = await _tagRepository.GetByNameAsync(normalizedName);
            if (existingTag != null)
            {
                _logger.LogError("Attempt to create tag with duplicate name: {tagName}", normalizedName);
                throw new InvalidOperationException($"Tag '{normalizedName}' already exists.");
            }

            var tag = new Tag(normalizedName);
            await _tagRepository.AddAsync(tag);

            var response = tag.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.TagCreated.ToString(), response);
            _logger.LogInformation("Tag created successfully.");
            return response;
        }

        public async Task DeleteTagAsync(DeleteTagRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete tag with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete tag request can not be null.");
            }

            var tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                _logger.LogError("Attempt to delete non existing tag. Tag id: {tagId}", request.Id);
                throw new KeyNotFoundException($"Tag with id {request.Id} not found.");
            }

            await _tagRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.TagDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Tag deleted successfully.");
        }

        public async Task<ICollection<TagResponseDTO>?> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            if (tags == null)
            {
                _logger.LogInformation("No tags to fetch from database.");
                return null;
            }

            _logger.LogInformation("Successfully fetched {count} tags from database.", tags.Count);
            return tags.Select(tag => tag.ToResponse()).ToList();
        }

        public async Task<TagResponseDTO> GetTagByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Attempt to retrieve tag with empty id.");
                throw new ArgumentException("Tag id can not be empty.", nameof(id));
            }

            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                _logger.LogError("Attempt to retrieve non existing tag. Tag id: {tagId}", id);
                throw new KeyNotFoundException($"Tag with id {id} not found.");
            }

            _logger.LogInformation("Tag with id {id} retrieved successfully.", id);
            return tag.ToResponse();
        }

        public async Task<TagResponseDTO> UpdateTagAsync(UpdateTagRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update tag with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update tag request can not be null.");
            }

            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                _logger.LogError("Attempt to update tag name with empty. Tag id: {tagId}", request.Id);
                throw new ArgumentException("Tag name is required.", nameof(request.Name));
            }

            var tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                _logger.LogError("Attempt to update non existing tag. Tag id: {tagId}", request.Id);
                throw new KeyNotFoundException($"Tag with id {request.Id} not found.");
            }

            var duplicateTag = await _tagRepository.GetByNameAsync(normalizedName);
            if (duplicateTag != null && duplicateTag.Id != tag.Id)
            {
                _logger.LogError("Attempt to update tag with duplicate name: {tagName}", normalizedName);
                throw new InvalidOperationException($"Tag '{normalizedName}' already exists.");
            }

            tag.Name = normalizedName;
            await _tagRepository.UpdateAsync(tag);

            var response = tag.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.TagUpdated.ToString(), response);
            _logger.LogInformation("Tag updated successfully.");
            return response;
        }
    }
}
