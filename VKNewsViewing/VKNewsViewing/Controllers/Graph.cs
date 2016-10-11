using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class Graph
    {
        private VkApi _api;
        public Dictionary<int, Task<List<UserModel>>> GraphUsers { get; }
        public List<PostsCollection> SortedPosts { get; set; }

        public Graph()
        {
            GraphUsers = new Dictionary<int, Task<List<UserModel>>>();
            _api = new VkApi();
            SortedPosts = new List<PostsCollection>();
        }

        private async Task<List<UserModel>> GetFriendsForMemberAsync(int userId)
        {
            var userFriends = await _api.GetFriendsAsync(userId);
            return userFriends?.items;
        }

        public async Task<Dictionary<int, Task<List<UserModel>>>> GetAllFriendsAsync(List<UserModel> members)
        {
            foreach (var member in members)
            {
                var friends = GetFriendsForMemberAsync(member.id);
                GraphUsers.Add(member.id, friends);
            }
            await Task.WhenAll(GraphUsers.Values);
            //foreach (var member in members)
            //{
            //    var friendsId = await GetFriendsAsync(member.id);
            //    if (friendsId == null) continue;
            //    GraphUsers.Add(member.id, friendsId);
            //}
            return GraphUsers;
        }

        public async Task<UsersCollection> GetMembersAsync(int groupId)
        {
            return await _api.GetMembersAsync(groupId);
        }

        private async Task<PostsCollection> GetPostsForMemberFriendAsync(int friendId)
        {
            //var obj = new PostsCollection();
            //obj.count = 100;
            //return obj;
            var allTasks = new List<Task<PostsCollection>>();
            var allPosts = new PostsCollection();

            //var firstHundredPosts = _api.GetPostsAsync(friendId, 0);
            //allTasks.Add(firstHundredPosts);
            //var amountPosts = firstHundredPosts.count + 100;
            for (var i = 0; i < 300; i += 100)
            {
                var hundredPosts = _api.GetPostsAsync(friendId, i);
                allTasks.Add(hundredPosts);
            }

            await Task.WhenAll(allTasks);

            foreach (var post in allTasks)
            {
                allPosts.CopyData(post.Result.items);
            }
            return allPosts;
        }

        public async Task<List<PostsCollection>> GetAllPostsForUserAsync(int userId)
        {
            var allTasks = new List<Task<PostsCollection>>();

            foreach (var friend in GraphUsers[userId].Result)
            {
                allTasks.Add(GetPostsForMemberFriendAsync(friend.id));
            }
            await Task.WhenAll(allTasks);

            foreach (var post in allTasks)
            {
                SortedPosts.Add(post.Result);
            }
            //sorting
            return SortedPosts;
        }
    }
}