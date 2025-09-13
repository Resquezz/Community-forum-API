using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class DeletePostRequest
    {
        public DeletePostRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
