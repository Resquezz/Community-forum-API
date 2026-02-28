using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreatePostRequest
    {
        public CreatePostRequest(string content, Guid topicId)
        {
            Content = content;
            TopicId = topicId;
        }

        [Required]
        [StringLength(300)]
        public string Content { get; }

        [Required]
        public Guid TopicId { get; }
    }
}
