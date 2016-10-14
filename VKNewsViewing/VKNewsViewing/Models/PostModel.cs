using System.Collections.Generic;

namespace VKNewsViewing.Models
{
    public struct Likes
    {
        public int count;
    }

    public struct Comments
    {
        public int count;
    }

    public struct Reposts
    {
        public int count;
    }

    public class PostModel
    {
        public int owner_id;
        public int from_id;
        public int id;
        public Likes likes;
        public Comments comments;
        public Reposts reposts;
        public double date;
        public string text;
    }

    public class PostsCollection
    {
        public int count;
        public List<PostModel> items { get; set; }
        public PostsCollection()
        {
            items = new List<PostModel>();
        }

        public void CopyData(List<PostModel> posts)
        {
            if (posts != null && posts.Count > 0)
            {
                items.AddRange(posts);
            }
        }
    }
}