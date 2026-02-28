using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class DeletePostTagRequest
    {
        public DeletePostTagRequest(Guid postId, Guid tagId)
        {
            PostId = postId;
            TagId = tagId;
        }

        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid TagId { get; set; }
    }
}
