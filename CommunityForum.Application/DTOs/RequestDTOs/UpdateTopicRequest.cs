using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateTopicRequest
    {
        public UpdateTopicRequest(Guid id, string title, string description)
            : this(id, title, description, Guid.Empty)
        {
        }

        public UpdateTopicRequest(Guid id, string title, string description, Guid categoryId)
        {
            Id = id;
            Title = title;
            Description = description;
            CategoryId = categoryId;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}
