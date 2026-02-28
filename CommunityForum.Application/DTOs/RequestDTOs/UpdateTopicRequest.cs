using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public Guid CategoryId { get; set; }
    }
}
