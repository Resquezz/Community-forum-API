using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class LoginUserRequest
    {
        public LoginUserRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 12)]
        public string Password { get; set; }
    }
}
