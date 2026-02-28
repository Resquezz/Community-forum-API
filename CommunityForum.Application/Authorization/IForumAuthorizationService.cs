using CommunityForum.Domain.Enums;

namespace CommunityForum.Application.Authorization
{
    public interface IForumAuthorizationService
    {
        void EnsureCanManageOwnedEntity(Guid ownerId, string entityName);
        void EnsureCanManageTaxonomy(string entityName);
        void EnsureCanManageUserAccount(Guid targetUserId);
        Role GetCurrentRole();
        Guid GetCurrentUserId();
        bool IsModeratorOrAdmin();
    }
}