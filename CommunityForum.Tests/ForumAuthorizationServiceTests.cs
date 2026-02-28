using CommunityForum.Application.Authorization;
using CommunityForum.Domain.Enums;
using CommunityForum.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.Security.Claims;

namespace CommunityForum.Tests
{
    [TestFixture]
    public class ForumAuthorizationServiceTests
    {
        private static ForumAuthorizationService CreateService(Guid userId, Role role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            };

            var accessor = new HttpContextAccessor { HttpContext = context };
            return new ForumAuthorizationService(accessor);
        }

        [Test]
        public void GetCurrentUserId_ReturnsUserId_FromClaims()
        {
            var userId = Guid.NewGuid();
            var cut = CreateService(userId, Role.User);

            var result = cut.GetCurrentUserId();

            Assert.That(result, Is.EqualTo(userId));
        }

        [Test]
        public void GetCurrentUserId_ThrowsForbiddenException_IfClaimMissing()
        {
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth"))
            };
            var cut = new ForumAuthorizationService(new HttpContextAccessor { HttpContext = context });

            var exception = Assert.Throws<ForbiddenException>(() => cut.GetCurrentUserId());

            Assert.That(exception!.Message, Does.StartWith("User identifier is missing in token."));
        }

        [Test]
        public void EnsureCanManageOwnedEntity_ThrowsForbiddenException_ForAnotherOwnerAndUserRole()
        {
            var cut = CreateService(Guid.NewGuid(), Role.User);

            var exception = Assert.Throws<ForbiddenException>(() => cut.EnsureCanManageOwnedEntity(Guid.NewGuid(), "post"));

            Assert.That(exception!.Message, Does.StartWith("You can modify only your own post."));
        }

        [Test]
        public void EnsureCanManageOwnedEntity_DoesNotThrow_ForAdminRole()
        {
            var cut = CreateService(Guid.NewGuid(), Role.Admin);

            Assert.DoesNotThrow(() => cut.EnsureCanManageOwnedEntity(Guid.NewGuid(), "post"));
        }

        [Test]
        public void EnsureCanManageTaxonomy_ThrowsForbiddenException_ForUserRole()
        {
            var cut = CreateService(Guid.NewGuid(), Role.User);

            var exception = Assert.Throws<ForbiddenException>(() => cut.EnsureCanManageTaxonomy("tags"));

            Assert.That(exception!.Message, Does.StartWith("Only moderator or admin can manage tags."));
        }

        [Test]
        public void EnsureCanManageUserAccount_ThrowsForbiddenException_ForAnotherUserAndNonAdmin()
        {
            var cut = CreateService(Guid.NewGuid(), Role.User);

            var exception = Assert.Throws<ForbiddenException>(() => cut.EnsureCanManageUserAccount(Guid.NewGuid()));

            Assert.That(exception!.Message, Does.StartWith("Only admin can manage other users accounts."));
        }
    }
}
