using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Entities
{
    public class Vote
    {
        public Vote(Guid userId, VoteType voteType, Guid? postId = null, Guid? commentId = null)
        {
            UserId = userId;
            PostId = postId;
            CommentId = commentId;
            VoteType = voteType;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid? PostId { get; set; }
        public Post? Post { get; set; }
        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
        public int NumericValue => VoteType == VoteType.UpVote ? 1 : -1;
    }
}
