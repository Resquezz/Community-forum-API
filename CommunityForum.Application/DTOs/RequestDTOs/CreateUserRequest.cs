using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateUserRequest
    {
        public CreateUserRequest(string username, string email)
        {
            Username = username;
            Email = email;
        }

        public string Username { get; }
        public string Email { get; }
    }
}
