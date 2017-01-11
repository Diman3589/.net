namespace VkClient.Models
{
    public class VkClientPostModel
    {
        public int PostId;
        public int OwnerId;
        public int FromId;
        public double Date;
        public string Text;
        public double Likes;
        public double Reposts;
        public double Comments;
        public int SourcePostId;
        public string Attachments;
    }
}
