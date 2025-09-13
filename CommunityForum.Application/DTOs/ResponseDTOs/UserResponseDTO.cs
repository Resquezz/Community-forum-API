using CommunityForum.Domain.Enums;
using System;
using System.Collections.Generic;
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

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
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
