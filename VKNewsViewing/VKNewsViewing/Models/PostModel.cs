using System.Collections.Generic;

namespace VKNewsViewing.Models
{
    public class PostModel
    {
        public int owner_id;
        public int from_id;
        public int id;
    }

    public class PostsCollection
    {
        public int count;
        public List<PostModel> items { get; set; }

        public void CopyData(List<PostModel> posts)
        {
            items.AddRange(posts);
        }
    }
}