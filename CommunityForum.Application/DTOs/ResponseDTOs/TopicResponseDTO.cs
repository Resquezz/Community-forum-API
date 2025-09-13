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
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is not TopicResponseDTO other)
                return false;
            return Id == other.Id && Title == other.Title && Description == other.Description;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description);
        }
    }
}
