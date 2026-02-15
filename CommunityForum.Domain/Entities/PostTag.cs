using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class PostTag
    {
        public PostTag(Guid postId, Guid tagId)
        {
            PostId = postId;
            TagId = tagId;
        }

        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public Guid TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
