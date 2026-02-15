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

namespace CommunityForum.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ForumAuthorizationService? _authorizationService;
        private readonly ILogger<CommentService> _logger;
        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository,
            IHubContext<ForumHub> hubContext, ILogger<CommentService> logger, ForumAuthorizationService? authorizationService = null)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public async Task<CommentResponseDTO> CreateCommentAsync(CreateCommentRequest request)
        {
            if(request == null)
            {
                _logger.LogError("Attempt to create comment with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create comment request can not be null.");
            }
            if(string.IsNullOrWhiteSpace(request.Content))
            {
                _logger.LogError("Attempt to create comment without content. User id: {userId}, post id: {postId}, " +
                    "parent comment id: {parentCommentId}", request.UserId, request.PostId, request.ParentCommentId);
                throw new ArgumentException("Comment content is required.", nameof(request.Content));
            }
            if(request.UserId == Guid.Empty)
            {
                _logger.LogError("Attempt to create comment without user id. Post id: {postId}, parent comment id: " +
                    "{parentCommentId}", request.PostId, request.ParentCommentId);
                throw new ArgumentNullException(nameof(request.UserId), "User is required.");
            }
            _authorizationService?.EnsureCurrentUserMatches(request.UserId);
            if (request.PostId == Guid.Empty)
            {
                _logger.LogError("Attempt to create comment without post id. User id: {userId}, parent comment id: " +
                    "{parentCommentId}", request.UserId, request.ParentCommentId);
                throw new ArgumentNullException(nameof(request.PostId), "Post is required.");
            }

            var post = await _postRepository.GetByIdAsync(request.PostId);
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if(post == null || user == null)
            {
                _logger.LogError("Can not find user or post in database. User id: {userId}, post id: {postId}, parent comment " +
                    "id: {parentCommentId}", request.UserId, request.PostId, request.ParentCommentId);
                throw new KeyNotFoundException("Can not find user or post.");
            }

            Comment? parentComment = null;
            if(request.ParentCommentId.HasValue)
            {
                parentComment = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if(parentComment == null)
                {
                    _logger.LogError("Can not find parent comment in database. Parent comment id: {parentCommentId}",
                        request.ParentCommentId);
                    throw new KeyNotFoundException("Can not find parent comment.");
                }
            }

            var comment = new Comment(request.Content, user.Id, post.Id, parentComment?.Id);
            await _commentRepository.AddAsync(comment);
            var response = comment.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.CommentCreated.ToString(), response);
            _logger.LogInformation("Comment created successfully.");
            return response;
        }

        public async Task DeleteCommentAsync(DeleteCommentRequest request)
        {
            if(request == null)
            {
                _logger.LogError("Attempt to delete comment with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete comment request can not be null.");
            }
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                _logger.LogError("Attempt to delete non existing comment. Comment id: {commentId}", request.Id);
                throw new KeyNotFoundException($"Comment with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(comment.UserId, "comment");

            await _commentRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.CommentDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Comment deleted successfully.");
        }

        public async Task<CommentResponseDTO> UpdateCommentAsync(UpdateCommentRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update comment with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update comment request can not be null.");
            }
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                _logger.LogError("Attempt to update non existing comment. Comment id: {commentId}", request.Id);
                throw new KeyNotFoundException($"Comment with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(comment.UserId, "comment");

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                _logger.LogError("Attempt to update comment content with empty. Comment id: {commentId}", request.Id);
                throw new ArgumentException("Comment content is required.", nameof(request.Content));
            }

            comment.Content = request.Content;
            await _commentRepository.UpdateAsync(comment);
            var response = comment.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.CommentUpdated.ToString(), response);
            _logger.LogInformation("Comment updated successfully.");
            return response;
        }
        public async Task<ICollection<CommentResponseDTO>>? GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            if (comments == null)
            {
                _logger.LogInformation("No comments to fetch from database.");
                return null!;
            }
            else
            {
                _logger.LogInformation("Successfully fetched {count} comments from database.", comments.Count);
                return comments.Select(comment => comment.ToResponse()).ToList();
            }
        }
        public async Task<CommentResponseDTO> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                _logger.LogError("Atempt to retrieve non existing comment. Comment id: {commentId}", id);
                throw new KeyNotFoundException($"Comment with id {id} not found.");
            }
            _logger.LogInformation("Comment with id {id} retrieved successfully.", id);
            return comment.ToResponse();
        }
    }
}
