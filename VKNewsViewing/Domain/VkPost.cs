using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class VkPost
    {
        [Key, Column(Order = 0)]
        public int OwnerId { get; set; }

        [Key, Column(Order = 1)]
        public int PostId { get; set; }

        public string Text { get; set; }
        public long Date { get; set; }
        public string Attachments { get; set; }
        public string Url { get; set; }
        public int RepostId { get; set; }
        public int FromId { get; set; }
        public double Likes { get; set; }
        public double Comments { get; set; }
        public double Shares { get; set; }
        public double LikesRating { get; set; }
        public double SharesRating { get; set; }
        public double CommentsRating { get; set; }

        public VkUser VkUserId { get; set; }
    }
}