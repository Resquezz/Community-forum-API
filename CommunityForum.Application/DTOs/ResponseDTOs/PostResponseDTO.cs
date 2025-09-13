using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class PostResponseDTO
    {
        public PostResponseDTO(Guid id, string content, Guid userId, string username, Guid topicId, string topicTitle)
        {
            Id = id;
            Content = content;
            UserId = userId;
            Username = username;
            TopicId = topicId;
            TopicTitle = topicTitle;
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid TopicId { get; set; }
        public string TopicTitle { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is not PostResponseDTO other)
                return false;
            return Id == other.Id && Content == other.Content && UserId == other.UserId
                && Username == other.Username && TopicId == other.TopicId && TopicTitle == other.TopicTitle;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Content, UserId, Username, TopicId, TopicTitle);
        }
    }
}
