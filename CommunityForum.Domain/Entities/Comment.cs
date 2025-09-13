using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class Comment
    {
        public Comment(string content, Guid userId, Guid postId, Guid? parentCommentId = null)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            UserId = userId;
            PostId = postId;
            ParentCommentId = parentCommentId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
