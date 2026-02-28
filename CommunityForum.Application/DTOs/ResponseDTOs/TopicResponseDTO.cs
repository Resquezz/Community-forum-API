using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class TopicResponseDTO
    {
        public TopicResponseDTO(Guid id, string title, string description)
            : this(id, title, description, Guid.Empty, string.Empty, DateTime.MinValue)
        {
        }

        public TopicResponseDTO(Guid id, string title, string description, Guid categoryId, string categoryName,
            DateTime createdAt = default)
        {
            Id = id;
            Title = title;
            Description = description;
            CategoryId = categoryId;
            CategoryName = categoryName;
            CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CategoryName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is not TopicResponseDTO other)
                return false;
            return Id == other.Id && Title == other.Title && Description == other.Description
                && CategoryId == other.CategoryId && CategoryName == other.CategoryName
                && CreatedAt == other.CreatedAt;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, CategoryId, CategoryName, CreatedAt);
        }
    }
}
