using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Newtonsoft.Json.Linq;
using VkClient.Models;
using Orm;

namespace PostsHandler
{
    public class VkPostsPreparer
    {
        private readonly FbClient.Client _fbClient;

        public VkPostsPreparer()
        {
            _fbClient = new FbClient.Client();
        }

        public async Task<List<VkPost>> PreparePosts(List<VkClientPostModel> allPosts)
        {
            var vkPosts = new List<VkPost>();
            foreach (var post in allPosts)
            {
                var vkPost = Mapper.MapVkPostModel(post);

                vkPost.Url = ParseUrl(post.Attachments);
                CalculateRating(post.OwnerId, post, ref vkPost);

                vkPosts.Add(vkPost);
            }

            var fbPosts = vkPosts.Select(p => p).Where(post => post.Url != null).ToList();
            vkPosts.RemoveAll(p => p.Url != null);

            var editedFbPosts = await GetRatingFbPosts(fbPosts);
            vkPosts.AddRange(editedFbPosts);

            return vkPosts;
        }

        private DateTime ConvertDate(double date)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(date);
        }

        private static string ParseUrl(string attachments)
        {
            if (attachments == null)
                return null;
            var result = JObject.Parse(attachments);
            var arrayAttachments = (JArray) result["attachments"];
            string url = null;
            foreach (var attach in arrayAttachments)
            {
                var type = attach["type"].ToString();
                if (type != "link" || ((string) attach["link"]["url"]).Contains("vk.com")) continue;
                url = attach["link"]["url"].ToString();
                break;
            }
            return url;
        }

        private async Task<List<VkPost>> GetRatingFbPosts(List<VkPost> posts)
        {
            var urls = posts.Select(p => p.Url).ToList();
            var fbPosts = await _fbClient.GetAllFbPostsInfo(urls);
            posts.ForEach(p =>
            {
                var post = fbPosts.FirstOrDefault(fb => fb.Id == p.Url);
                if (post == null) return;
                p.Shares = post.Shares;
                p.Comments = post.Comments;
            });

            return posts;
        }

        private static void CalculateRating(int userId, VkClientPostModel post, ref VkPost vkPost)
        {
            var orm = new OrmWorker();
            var user = orm.GetUserById(userId);

            if (vkPost.Url != null || userId < 0 || user == null)
            {
                vkPost.LikesRating = 0;
                vkPost.SharesRating = 0;
                vkPost.CommentsRating = 0;
                return;
            }

            vkPost.LikesRating = double.IsNaN(post.Likes/user.FriendsCount) ||
                                 double.IsInfinity(post.Likes/user.FriendsCount)
                ? 0
                : Math.Round(post.Likes/user.FriendsCount, 3, MidpointRounding.AwayFromZero);
            
            vkPost.SharesRating = double.IsNaN(post.Reposts/user.FriendsCount) ||
                                  double.IsInfinity(post.Reposts/user.FriendsCount)
                ? 0
                : Math.Round(post.Reposts/user.FriendsCount, 3, MidpointRounding.AwayFromZero);

            vkPost.CommentsRating = double.IsNaN(post.Comments/user.FriendsCount) ||
                                    double.IsInfinity(post.Comments/user.FriendsCount)
                ? 0
                : Math.Round(post.Comments/user.FriendsCount, 3, MidpointRounding.AwayFromZero);
        }
    }
}