using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class VkApi
    {
        private string ver = "&v=5.57";

        private void RemoveNullUsers(ref UsersCollection userses)
        {
            userses.items.RemoveAll(user => user.deactivated == "deleted");
            userses.count = userses.items.Count;
        }

        private string RemoveRoot(string json)
        {
            var obj = JObject.Parse(json);
            var token = obj["response"];
            return token?.ToString();
        }

        private async Task<string> MakeRequestAsync(string url, int id, string fields, string parameters = "")
        {
            fields = "&fields=" + fields;
            var req = WebRequest.Create(url + id + parameters + fields + ver);
            var resp = await req.GetResponseAsync();
            var sr = new StreamReader(resp.GetResponseStream());
            var json = sr.ReadToEnd();
            resp.Close();
            return json;
        }

        public async Task<UsersCollection> GetMembersAsync(int groupId)
        {
            var json =
                await MakeRequestAsync("https://api.vk.com/method/groups.getMembers?group_id=", groupId, "nickname");
            json = RemoveRoot(json);
            if (json == null)
                return null;
            var members = JsonConvert.DeserializeObject<UsersCollection>(json);
            RemoveNullUsers(ref members);
            return members;
            //var obj = new
            //{
            //    count = 0,
            //    items = new[]
            //    {
            //        new
            //        {
            //            id = 0,
            //            first_name = "",
            //            last_name = "",
            //            nickname = "",
            //            hidden = 0
            //        }
            //    }
            //};
            //var result = JsonConvert.DeserializeAnonymousType(json, obj);
            //var data1 = result.items;
        }

        public async Task<UsersCollection> GetFriendsAsync(int userId)
        {
            var json = await MakeRequestAsync("https://api.vk.com/method/friends.get?user_id=", userId, "nickname");
            json = RemoveRoot(json);
            return json != null ? JsonConvert.DeserializeObject<UsersCollection>(json) : null;
        }

        public async Task<PostsCollection> GetPostsAsync(int userId, int offset = 0)
        {
            var json = await MakeRequestAsync("https://api.vk.com/method/wall.get?owner_id=", userId, "nickname",
                "&filter=owner&count=100&offset=" + offset);
            json = RemoveRoot(json);
            //if (json != null)
            //{
            //    var obj = JObject.Parse(json);
            //    json = obj["items"]?.ToString();
            //}
            return json != null ? JsonConvert.DeserializeObject<PostsCollection>(json) : null;
        }
    }
}