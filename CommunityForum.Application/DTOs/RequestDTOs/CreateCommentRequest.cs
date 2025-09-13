using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateCommentRequest
    {
        public CreateCommentRequest(string content, Guid userId, Guid postId, Guid? parentCommentId = null)
        {
            Content = content;
            UserId = userId;
            PostId = postId;
            ParentCommentId = parentCommentId;
        }

        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
