using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class Post
    {
        public Post(string content, Guid userId, Guid topicId)
        {
            Content = content;
            UserId = userId;
            TopicId = topicId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid TopicId { get; set; }
        public Topic? Topic { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
