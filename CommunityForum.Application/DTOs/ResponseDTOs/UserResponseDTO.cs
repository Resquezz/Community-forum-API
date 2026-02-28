using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public UserResponseDTO(Guid id, string username, string email, Role role)
        {
            Id = id;
            Username = username;
            Email = email;
            Role = role;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EnumDataType(typeof(Role), ErrorMessage = "Such role does not exist!")]
        public Role Role { get; set; } = Role.User;
        public override bool Equals(object? obj)
        {
            if(obj is not UserResponseDTO other)
                return false;
            return Id == other.Id && Username == other.Username && Email == other.Email && Role == other.Role;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Username, Email, Role);
        }
    }
}
