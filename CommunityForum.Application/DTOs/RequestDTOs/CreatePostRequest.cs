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
        public CreatePostRequest(string content, Guid userId, Guid topicId)
        {
            Content = content;
            UserId = userId;
            TopicId = topicId;
        }

        [Required]
        [StringLength(300)]
        public string Content { get; }

        [Required]
        public Guid UserId { get; }

        [Required]
        public Guid TopicId { get; }
    }
}
