using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateCommentRequest
    {
        public UpdateCommentRequest(Guid id, string content)
        {
            Id = id;
            Content = content;
        }

        [Required]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string Content { get; set; }
    }
}
