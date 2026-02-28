using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class PostTagResponseDTO
    {
        public PostTagResponseDTO(Guid postId, Guid tagId, string tagName)
        {
            PostId = postId;
            TagId = tagId;
            TagName = tagName;
        }

        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid TagId { get; set; }

        [Required]
        [StringLength(100)]
        public string TagName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not PostTagResponseDTO other)
                return false;
            return PostId == other.PostId && TagId == other.TagId && TagName == other.TagName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PostId, TagId, TagName);
        }
    }
}
