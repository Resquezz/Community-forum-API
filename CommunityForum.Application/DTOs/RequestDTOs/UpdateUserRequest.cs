using System;
using System.Collections.Generic;
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

        public Guid Id { get; }
        public string Username { get; }
        public string Email { get; }
    }
}
