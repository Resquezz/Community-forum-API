using System;
using System.Collections.Generic;
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

        public string Content { get; }
        public Guid UserId { get; }
        public Guid TopicId { get; }
    }
}
