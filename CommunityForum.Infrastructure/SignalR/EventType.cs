using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Infrastructure.SignalR
{
    public enum EventType
    {
        PostCreated,
        PostUpdated,
        PostDeleted,

        CommentCreated,
        CommentUpdated,
        CommentDeleted,

        TopicCreated,
        TopicUpdated,
        TopicDeleted,
        
        VoteCreated,
        VoteUpdated,
        VoteDeleted
    }
}
