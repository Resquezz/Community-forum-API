using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateTagRequest
    {
        public UpdateTagRequest(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [Required]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}
