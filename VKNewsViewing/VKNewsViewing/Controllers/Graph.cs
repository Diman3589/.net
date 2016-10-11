using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class Graph
    {
        private VkApi _api;
        public Dictionary<int, List<UserModel>> GraphUsers { get; set; }
        public PostsCollection SortedPosts { get; }

        public Graph()
        {
            GraphUsers = new Dictionary<int, List<UserModel>>();
            _api = new VkApi();
            SortedPosts = new PostsCollection {items = new List<PostModel>(), count = 0};
        }

        private async Task<List<UserModel>> GetFriendsForMemberAsync(int userId)
        {
            var userFriends = await _api.GetFriendsAsync(userId);
            return userFriends?.items;
        }

        public async Task<Dictionary<int, List<UserModel>>> GetAllFriendsAsync(List<UserModel> members)
        {
            var tasks = new Dictionary<int, Task<List<UserModel>>>();
            foreach (var member in members)
            {
                var friends = GetFriendsForMemberAsync(member.id);
                tasks.Add(member.id, friends);
            }
            await Task.WhenAll(tasks.Values);

            foreach (var user in GraphUsers)
            {
                GraphUsers.Add(user.Key, user.Value);
            }
            return GraphUsers;
        }

        public async Task<UsersCollection> GetMembersAsync(int groupId)
        {
            return await _api.GetMembersAsync(groupId);
        }

        private async Task<PostsCollection> GetPostsForMemberFriendAsync(int friendId)
        {
            var allPosts = new PostsCollection {items = new List<PostModel>(), count = 0};
            var hundredPosts = await _api.GetPostsAsync(friendId);
            var i = 0;
            while (hundredPosts != null && hundredPosts.items.Count != 0)
            {
                allPosts.CopyData(hundredPosts.items);
                allPosts.count = hundredPosts.count;
                i += 100;
                hundredPosts = await _api.GetPostsAsync(friendId, i);
            }
            return allPosts;
        }

        public async Task<PostsCollection> GetAllPostsForUserAsync(int userId)
        {
            var allTasks = new List<Task<PostsCollection>>();

            foreach (var friend in GraphUsers[userId])
            {
                allTasks.Add(GetPostsForMemberFriendAsync(friend.id));
            }

            var results = await Task.WhenAll(allTasks);

            var allPosts = new PostsCollection {items = new List<PostModel>(), count = 0};

            foreach (var post in results)
            {
                if (post.count != 0)
                {
                    allPosts.CopyData(post.items);
                    allPosts.count += post.count;
                }
            }
            return allPosts;
        }

        public PostsCollection SortPosts(string filter, PostsCollection allPosts)
        {
            switch (filter)
            {
                case "likes":
                    SortedPosts.items = allPosts.items.OrderByDescending(post => post.likes.count).ToList();
                    break;
                case "comments":
                    SortedPosts.items = allPosts.items.OrderByDescending(post => post.comments.count).ToList();
                    break;
                case "reposts":
                    SortedPosts.items = allPosts.items.OrderByDescending(post => post.reposts.count).ToList();
                    break;
            }
            SortedPosts.count = allPosts.count;
            return SortedPosts;
        }
    }
}