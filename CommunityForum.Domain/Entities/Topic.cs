using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class Topic
    {
        public Topic(string title, string description)
            : this(title, description, Guid.Empty, Guid.Empty)
        {
        }

        public Topic(string title, string description, Guid categoryId)
            : this(title, description, categoryId, Guid.Empty)
        {
        }

        public Topic(string title, string description, Guid categoryId, Guid userId)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CategoryId = categoryId;
            UserId = userId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
