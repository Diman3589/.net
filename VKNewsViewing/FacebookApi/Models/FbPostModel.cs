namespace FacebookApi.Models
{
    public struct Counter
    {
        public int comment_count;
        public int share_count;
    }

    public class FbPostModel
    {
        public Counter share;
        public string id;
        public string description;
        public string title;

    }
}
