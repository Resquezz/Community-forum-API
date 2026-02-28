using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public Guid Id { get; }

        [StringLength(300)]
        public string Content { get; }
    }
}
