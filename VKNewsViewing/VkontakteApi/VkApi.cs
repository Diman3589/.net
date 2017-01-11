using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkontakteApi.Models;

namespace VkontakteApi
{
    public static class VkApi
    {
        private const string ver = "&v=5.58";

        private static void RemoveDeletedUsers(ref UsersCollection users)
        {
            users.items.RemoveAll(user => user.deactivated != null);
            users.count = users.items.Count;
        }

        private static void RemoveHiddenUsers(ref UsersCollection users)
        {
            users.items.RemoveAll(user => user.hidden == 1);
            users.count = users.items.Count;
        }

        private static string RemoveRoot(string json)
        {
            var obj = JObject.Parse(json);
            var token = obj["response"];
            return token?.ToString();
        }

        public static async Task<string> PerformRequestAsync(string url, string fields, string parameters = "")
        {
            var req = WebRequest.Create($"{url}{parameters}&fields={fields}{ver}");
            try
            {
                using (var resp = await req.GetResponseAsync())
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var json = sr.ReadToEnd();
                        return json;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<UsersCollection> GetMembersAsync(int groupId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/groups.getMembers?group_id={groupId}&fields=", "photo");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            var members = JsonConvert.DeserializeObject<UsersCollection>(json);
            RemoveDeletedUsers(ref members);
            return members;
        }

        public static async Task<UsersCollection> GetFriendsAsync(int userId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/friends.get?user_id={userId}", "photo");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            var friends = JsonConvert.DeserializeObject<UsersCollection>(json);
            RemoveDeletedUsers(ref friends);
            RemoveHiddenUsers(ref friends);

            return friends;
        }

        public static async Task<PostsCollection> GetPostsAsync(int userId, int count = 30)
        {
            var json =
                await
                    PerformRequestAsync($"https://api.vk.com/method/wall.get?owner_id={userId}", "",
                        $"&filter=owner&count={count}");
            json = RemoveRoot(json);
            return json != null ? JsonConvert.DeserializeObject<PostsCollection>(json) : null;
        }

        public static async Task<int> GetCountFriendsAsync(int userId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/friends.get?user_id={userId}", "");
            json = RemoveRoot(json);
            if (json == null)
                return 0;
            dynamic res = JObject.Parse(json);
            return res.count;
        }

        public static async Task<string[]> GetGroupInfoAsync(int groupId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/groups.getById?group_id={groupId}", "");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            dynamic result = JObject.Parse(json);
            return new string[] {result.name, result.photo_50};
        }

        public static async Task<string[]> GetUserInfoAsync(int userId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/groups.getById?group_id={userId}", "photo");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            dynamic result = JObject.Parse(json);
            return new string[] {result.first_name + " " + result.last_name, result.photo};
        }

        public static async Task<Dictionary<string, int>> GetPostInfoAsync(int userId, int postId)
        {
            var json = await PerformRequestAsync($"https://api.vk.com/method/wall.getById?posts={userId}_{postId}", "");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            dynamic res = JObject.Parse(json);
            var postInfo = new Dictionary<string, int>
            {
                {"comments", res.comments.count},
                {"likes", res.likes.count},
                {"reposts", res.reposts.count}
            };

            return postInfo;
        }
    }
}