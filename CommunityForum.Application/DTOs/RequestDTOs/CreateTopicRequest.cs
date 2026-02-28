using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
