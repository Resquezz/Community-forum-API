using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateUserRequest
    {
        public UpdateUserRequest(Guid id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
        }

        [Required]
        public Guid Id { get; }

        [StringLength(50, MinimumLength = 3)]
        public string Username { get; }

        [EmailAddress]
        public string Email { get; }
    }
}
