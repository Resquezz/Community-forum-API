using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Authorization
{
    public class ForumAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForumAuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ForbiddenException("User identifier is missing in token.");
            }

            return userId;
        }

        public Role GetCurrentRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(roleClaim) || !Enum.TryParse<Role>(roleClaim, out var role))
            {
                throw new ForbiddenException("User role is missing in token.");
            }

            return role;
        }

        public bool IsModeratorOrAdmin()
        {
            var role = GetCurrentRole();
            return role == Role.Admin || role == Role.Moderator;
        }

        public void EnsureCanManageOwnedEntity(Guid ownerId, string entityName)
        {
            if (IsModeratorOrAdmin())
            {
                return;
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != ownerId)
            {
                throw new ForbiddenException($"You can modify only your own {entityName}.");
            }
        }

        public void EnsureCanManageTaxonomy(string entityName)
        {
            if (!IsModeratorOrAdmin())
            {
                throw new ForbiddenException($"Only moderator or admin can manage {entityName}.");
            }
        }

        public void EnsureCurrentUserMatches(Guid userId)
        {
            if (IsModeratorOrAdmin())
            {
                return;
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != userId)
            {
                throw new ForbiddenException("You can perform this action only for your own account.");
            }
        }

        public void EnsureCanManageUserAccount(Guid targetUserId)
        {
            var role = GetCurrentRole();
            if (role == Role.Admin)
            {
                return;
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != targetUserId)
            {
                throw new ForbiddenException("Only admin can manage other users accounts.");
            }
        }
    }
}
