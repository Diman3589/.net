using System.Collections.Generic;

namespace VkontakteApi.Models
{
    public struct Counter
    {
        public double count;
    }

    public struct PhotoType
    {
        public int id;
        public string photo_130;
        public string photo_75;
        public string photo_604;
        public string photo_807;
        public int width;
        public int height;
        public string text;
    }

    public class VideoType
    {
        public int id;
        public string title;
        public string photo_130;
        public string photo_320;
        public string photo_800;
        public int width;
        public int height;
    }

    public struct LinkType
    {
        public string url;
        public string title;
        public string caption;
        public string description;
        public PhotoType photo;
        public VideoType video;
        public AudioType audio;
    }

    public struct AudioType
    {
        public string artist;
        public string title;
        public int duration;
    }

    public struct Attachment
    {
        public string type;
        public PhotoType photo;
        public LinkType link;
        public VideoType video;
        public AudioType audio;
    }

    public class PostModel
    {
        public int id;
        public int owner_id;
        public int from_id;
        public double date;
        public string text;
        public List<PostModel> copy_history;
        public List<Attachment> attachments;
        public Counter likes;
        public Counter reposts;
        public Counter comments;
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