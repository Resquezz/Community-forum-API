using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class CategoryResponseDTO
    {
        public CategoryResponseDTO(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not CategoryResponseDTO other)
                return false;
            return Id == other.Id && Name == other.Name && Description == other.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description);
        }
    }
}
