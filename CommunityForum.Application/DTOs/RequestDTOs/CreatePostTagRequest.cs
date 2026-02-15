using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreatePostTagRequest
    {
        public CreatePostTagRequest(Guid postId, Guid tagId)
        {
            PostId = postId;
            TagId = tagId;
        }

        public Guid PostId { get; set; }
        public Guid TagId { get; set; }
    }
}
