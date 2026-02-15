using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateTopicRequest
    {
        public CreateTopicRequest(string title, string description)
            : this(title, description, Guid.Empty)
        {
        }

        public CreateTopicRequest(string title, string description, Guid categoryId)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}
