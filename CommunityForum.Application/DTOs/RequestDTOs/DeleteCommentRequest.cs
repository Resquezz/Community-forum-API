using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class DeleteCommentRequest
    {
        public DeleteCommentRequest(Guid id)
        {
            Id = id;
        }

        [Required]
        public Guid Id { get; set; }
    }
}
