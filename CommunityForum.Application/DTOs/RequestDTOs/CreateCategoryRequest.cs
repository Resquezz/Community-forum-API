using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateCategoryRequest
    {
        public CreateCategoryRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
    }
}
