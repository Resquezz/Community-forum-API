using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class Tag
    {
        public Tag(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
