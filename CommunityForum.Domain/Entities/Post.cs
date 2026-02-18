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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid TopicId { get; set; }
        public Topic? Topic { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public int UpvotesCount => Votes.Count(v => v.VoteType == VoteType.UpVote);
        public int DownvotesCount => Votes.Count(v => v.VoteType == VoteType.DownVote);
        public int Score => UpvotesCount - DownvotesCount;
        public int CommentsCount => Comments.Count;
    }
}
