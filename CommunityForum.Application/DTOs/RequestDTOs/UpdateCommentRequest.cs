using System;
using System.Collections.Generic;
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

        public Guid Id { get; set; }
        public string Content { get; set; }
    }
}
