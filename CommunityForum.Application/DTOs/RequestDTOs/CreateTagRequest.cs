using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateTagRequest
    {
        public CreateTagRequest(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
