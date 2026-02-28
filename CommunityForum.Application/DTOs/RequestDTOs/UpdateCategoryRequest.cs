using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateCategoryRequest
    {
        public UpdateCategoryRequest(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        [Required]
        public Guid Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
    }
}
