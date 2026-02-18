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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly IUserRepository _userRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IHubContext<ForumHub> _hubContext;
        private readonly ForumAuthorizationService? _authorizationService;
        private readonly ILogger<VoteService> _logger;
        public VoteService(IUserRepository userRepository, IVoteRepository voteRepository, IPostRepository postRepository,
            ICommentRepository commentRepository, IHubContext<ForumHub> hubContext, ILogger<VoteService> logger,
            ForumAuthorizationService? authorizationService = null)
        {
            _userRepository = userRepository;
            _voteRepository = voteRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _hubContext = hubContext;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public async Task<VoteResponseDTO> CreateVoteAsync(CreateVoteRequest request)
        {
            if(request == null)
            {
                _logger.LogError("Attempt to create vote with null request instance.");
                throw new ArgumentNullException(nameof(request), "Create vote request can not be null.");
            }
            var user = await _userRepository.GetByIdAsync(request.UserId);
            Post? post = null;
            Comment? comment = null;
            if (user == null)
            {
                _logger.LogError("Attempt to create vote for non existing user. User id: {userId}", request.UserId);
                throw new KeyNotFoundException($"User with id {request.UserId} not found.");
            }
            _authorizationService?.EnsureCurrentUserMatches(request.UserId);
            if(request.PostId == null && request.CommentId == null)
            {
                _logger.LogError("Ivalid vote creation request. No postId nor commentId provided. User id: {userId}", request.UserId);
                throw new ArgumentNullException(nameof(request.PostId) + nameof(request.CommentId),
                    "Vote must be for a post or a comment.");
            }
            if(request.PostId != null)
            {
                post = await _postRepository.GetByIdAsync(request.PostId.Value);
                if (post == null)
                {
                    _logger.LogError("Attempt to vote for non existing post. Post id: {postId}, user id: {userId}",
                        request.PostId, request.UserId);
                    throw new KeyNotFoundException($"Post with id {request.PostId} not found.");
                }
            }
            if (request.CommentId != null)
            {
                comment = await _commentRepository.GetByIdAsync(request.CommentId.Value);
                if (comment == null)
                {
                    _logger.LogError("Attempt to vote for non existing comment. Comment id: {commentId}, user id: {userId}",
                        request.CommentId, request.UserId);
                    throw new KeyNotFoundException($"Comment with id {request.CommentId} not found.");
                }
            }

            var vote = new Vote(user.Id, request.VoteType, post?.Id, comment?.Id);
            await _voteRepository.AddAsync(vote);
            var response = vote.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.VoteCreated.ToString(), response);
            _logger.LogInformation("Vote created successfully.");
            return response;
        }

        public async Task DeleteVoteAsync(DeleteVoteRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete vote with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete vote request can not be null.");
            }
            var vote = await _voteRepository.GetByIdAsync(request.Id);
            if (vote == null)
            {
                _logger.LogError("Attempt to delete non existing vote. Vote id: {voteId}", request.Id);
                throw new KeyNotFoundException($"Vote with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(vote.UserId, "vote");

            await _voteRepository.DeleteAsync(request.Id);
            await _hubContext.Clients.All.SendAsync(EventType.VoteDeleted.ToString(), new { request.Id });
            _logger.LogInformation("Vote deleted successfully.");
        }

        public async Task<VoteResponseDTO> UpdateVoteAsync(UpdateVoteRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update vote with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update vote request can not be null.");
            }
            var vote = await _voteRepository.GetByIdAsync(request.Id);
            if (vote == null)
            {
                _logger.LogError("Attempt to update non existing vote. Vote id: {voteId}", request.Id);
                throw new KeyNotFoundException($"Vote with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageOwnedEntity(vote.UserId, "vote");

            vote.VoteType = request.VoteType;
            vote.VotedAt = DateTime.UtcNow;
            await _voteRepository.UpdateAsync(vote);
            var response = vote.ToResponse();
            await _hubContext.Clients.All.SendAsync(EventType.VoteUpdated.ToString(), response);
            _logger.LogInformation("Vote updated successfully.");
            return response;
        }
        public async Task<VoteResponseDTO> GetVoteByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Atempt to retrieve non existing vote. Vote id: {voteId}", id);
                throw new ArgumentNullException(nameof(id), "Vote id can not be empty.");
            }
            var vote = await _voteRepository.GetByIdAsync(id);
            if (vote == null)
            {
                _logger.LogError("Atempt to retrieve non existing vote. Vote id: {voteId}", id);
                throw new KeyNotFoundException($"Vote with id {id} not found.");
            }
            _logger.LogInformation("Vote with id {id} retrieved successfully.", id);
            return vote.ToResponse();
        }
    }
}
