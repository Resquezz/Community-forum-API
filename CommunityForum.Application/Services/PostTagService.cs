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
    public class PostTagService : IPostTagService
    {
        private readonly IPostTagRepository _postTagRepository;
        private readonly IPostRepository _postRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ILogger<PostTagService> _logger;

        public PostTagService(IPostTagRepository postTagRepository, IPostRepository postRepository, ITagRepository tagRepository,
            IHubContext<ForumHub> hubContext, ILogger<PostTagService> logger)
        {
            _postTagRepository = postTagRepository;
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<PostTagResponseDTO> AddPostTagAsync(CreatePostTagRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to create post-tag relation with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create post-tag request can not be null.");
            }
            if (request.PostId == Guid.Empty || request.TagId == Guid.Empty)
            {
                _logger.LogError("Attempt to create post-tag relation with empty identifiers. Post id: {postId}, tag id: {tagId}",
                    request.PostId, request.TagId);
                throw new ArgumentException("Post id and tag id are required.");
            }

            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                _logger.LogError("Attempt to create relation for non existing post. Post id: {postId}", request.PostId);
                throw new KeyNotFoundException($"Post with id {request.PostId} not found.");
            }

            var tag = await _tagRepository.GetByIdAsync(request.TagId);
            if (tag == null)
            {
                _logger.LogError("Attempt to create relation for non existing tag. Tag id: {tagId}", request.TagId);
                throw new KeyNotFoundException($"Tag with id {request.TagId} not found.");
            }

            var existingRelation = await _postTagRepository.GetByIdsAsync(request.PostId, request.TagId);
            if (existingRelation != null)
            {
                _logger.LogError("Attempt to create duplicate post-tag relation. Post id: {postId}, tag id: {tagId}",
                    request.PostId, request.TagId);
                throw new InvalidOperationException("Post-tag relation already exists.");
            }

            var postTag = new PostTag(request.PostId, request.TagId)
            {
                Tag = tag
            };
            await _postTagRepository.AddAsync(postTag);

            var response = postTag.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.PostTagCreated.ToString(), response);
            _logger.LogInformation("Post-tag relation created successfully.");
            return response;
        }

        public async Task DeletePostTagAsync(DeletePostTagRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete post-tag relation with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete post-tag request can not be null.");
            }

            var postTag = await _postTagRepository.GetByIdsAsync(request.PostId, request.TagId);
            if (postTag == null)
            {
                _logger.LogError("Attempt to delete non existing post-tag relation. Post id: {postId}, tag id: {tagId}",
                    request.PostId, request.TagId);
                throw new KeyNotFoundException("Post-tag relation not found.");
            }

            await _postTagRepository.DeleteAsync(request.PostId, request.TagId);
            await _hubContext.Clients.All.SendAsync(EventType.PostTagDeleted.ToString(), new { request.PostId, request.TagId });
            _logger.LogInformation("Post-tag relation deleted successfully.");
        }

        public async Task<ICollection<TagResponseDTO>> GetTagsByPostIdAsync(Guid postId)
        {
            if (postId == Guid.Empty)
            {
                _logger.LogError("Attempt to get tags for post with empty id.");
                throw new ArgumentException("Post id can not be empty.", nameof(postId));
            }

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogError("Attempt to get tags for non existing post. Post id: {postId}", postId);
                throw new KeyNotFoundException($"Post with id {postId} not found.");
            }

            var postTags = await _postTagRepository.GetByPostIdAsync(postId);
            _logger.LogInformation("Retrieved {count} tags for post {postId}.", postTags.Count, postId);
            return postTags.Where(postTag => postTag.Tag != null).Select(postTag => postTag.Tag!.ToResponse()).ToList();
        }
    }
}
