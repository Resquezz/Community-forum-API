using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class TopicResponseDTO
    {
        public TopicResponseDTO(Guid id, string title, string description)
            : this(id, title, description, Guid.Empty, string.Empty)
        {
        }

        public TopicResponseDTO(Guid id, string title, string description, Guid categoryId, string categoryName)
        {
            Id = id;
            Title = title;
            Description = description;
            CategoryId = categoryId;
            CategoryName = categoryName;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is not TopicResponseDTO other)
                return false;
            return Id == other.Id && Title == other.Title && Description == other.Description
                && CategoryId == other.CategoryId && CategoryName == other.CategoryName;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, CategoryId, CategoryName);
        }
    }
}
