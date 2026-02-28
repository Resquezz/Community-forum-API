using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [StringLength(200)]
        public string Content { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid PostId { get; set; }

        public Guid? ParentCommentId { get; set; }
    }
}
