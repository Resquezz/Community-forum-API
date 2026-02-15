using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Application.Authorization;
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
using System.Xml.Linq;

namespace CommunityForum.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ForumAuthorizationService? _authorizationService;
        private readonly ILogger<PostService> _logger;
        public PostService(IPostRepository postRepository, IUserRepository userRepository, ITopicRepository topicRepository,
            IHubContext<ForumHub> forumContext, ILogger<PostService> logger, ForumAuthorizationService? authorizationService = null)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _topicRepository = topicRepository;
            _hubContext = forumContext;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public async Task<PostResponseDTO> CreatePostAsync(CreatePostRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to create post with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create post request can not be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                _logger.LogError("Attempt to create post without content. User id: {userId}, topic id: {topicId}",
                    request.UserId, request.TopicId);
                throw new ArgumentException("Post content is required.", nameof(request.Content));
            }
            if (request.UserId == Guid.Empty)
            {
                _logger.LogError("Attempt to create post without user id.");
                throw new ArgumentNullException(nameof(request.UserId), "User is required.");
            }
            _authorizationService?.EnsureCurrentUserMatches(request.UserId);
            if (request.TopicId == Guid.Empty)
            {
                _logger.LogError("Attempt to create post without topic id.");
                throw new ArgumentNullException(nameof(request.TopicId), "Topic is required.");
            }
            var user = await _userRepository.GetByIdAsync(request.UserId);
            var topic = await _topicRepository.GetByIdAsync(request.TopicId);

            if (user == null || topic == null)
            {
                _logger.LogError("Can not find user or topic in database. User id: {userId}, topic id: {topicId}",
                    request.UserId, request.TopicId);
                throw new KeyNotFoundException("Can not find user or topic.");
            }

            Post post = new Post(request.Content, user.Id, topic.Id);

            await _postRepository.AddAsync(post);
            var response = post.ToResponse(user, topic);

            await _hubContext.Clients.All.SendAsync(EventType.PostCreated.ToString(), response);
            _logger.LogInformation("Post created successfully.");
            return response;
        }
        public async Task DeletePostAsync(DeletePostRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete post with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete post request can not be null.");
            }
            var post = await _postRepository.GetByIdAsync(request.Id);
            if (post == null)
            {
                _logger.LogError("Attempt to delete non existing post. Post id: {postId}", request.Id);
                throw new KeyNotFoundException($"Post with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(post.UserId, "post");

            await _postRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.PostDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Post deleted successfully.");
        }

        public async Task<PostResponseDTO> UpdatePostAsync(UpdatePostRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update post with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update post request can not be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                _logger.LogError("Attempt to update post content with empty. Post id: {postId}", request.Id);
                throw new ArgumentException("Post content is required.", nameof(request.Content));
            }
            var post = await _postRepository.GetByIdAsync(request.Id);
            if (post == null)
            {
                _logger.LogError("Attempt to update non existing post. Post id: {postId}", request.Id);
                throw new KeyNotFoundException($"Post with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(post.UserId, "post");

            var user = await _userRepository.GetByIdAsync(post.UserId);
            var topic = await _topicRepository.GetByIdAsync(post.TopicId);

            if (user == null || topic == null)
            {
                _logger.LogError("Can not find user or topic in database. User id: {userId}, topic id: {topicId}",
                    post.UserId, post.TopicId);
                throw new KeyNotFoundException("User or topic does not exist.");
            }

            post.Content = request.Content;
            await _postRepository.UpdateAsync(post);

            var response = post.ToResponse(user!, topic!);
            await _hubContext.Clients.All.SendAsync(EventType.PostUpdated.ToString(), response);
            _logger.LogInformation("Post updated successfully.");
            return response;
        }
        public async Task<ICollection<PostResponseDTO>?> GetAllPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            if(posts == null)
            {
                _logger.LogInformation("No posts to fetch from database.");
                return null;
            }
            _logger.LogInformation("Successfully fetched {count} posts from database.", posts.Count);
            var DTOs = new List<PostResponseDTO>();
            foreach(var post in posts)
            {
                var user = await _userRepository.GetByIdAsync(post.UserId);
                var topic = await _topicRepository.GetByIdAsync(post.TopicId);
                DTOs.Add(post.ToResponse(user!, topic!));
            }
            return DTOs;
        }
        public async Task<PostResponseDTO> GetPostByIdAsync(Guid id)
        {
            if(Guid.Empty == id)
            {
                _logger.LogError("Atempt to retrieve post with empty id.");
                throw new ArgumentException(nameof(id), "Post id can not be empty.");
            }
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                _logger.LogError("Atempt to retrieve non existing post. Post id: {postId}", id);
                throw new KeyNotFoundException($"Post with id {id} not found.");
            }
            var user = await _userRepository.GetByIdAsync(post.UserId);
            var topic = await _topicRepository.GetByIdAsync(post.TopicId);
            _logger.LogInformation("Post with id {id} retrieved successfully.", id);
            return post.ToResponse(user!, topic!);
        }
    }
}
