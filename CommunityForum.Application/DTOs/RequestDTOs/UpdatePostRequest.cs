using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdatePostRequest
    {
        public UpdatePostRequest(Guid id, string content)
        {
            Id = id;
            Content = content;
        }

        public Guid Id { get; }
        public string Content { get; }
    }
}
