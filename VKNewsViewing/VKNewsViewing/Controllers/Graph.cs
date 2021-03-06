﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class Graph
    {
        private VkApi _api;
        public Dictionary<int, List<UserModel>> GraphUsers { get; set; }
        public PostsCollection SortedPosts { get; private set; }

        public Graph()
        {
            GraphUsers = new Dictionary<int, List<UserModel>>();
            _api = new VkApi();
            SortedPosts = new PostsCollection();
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
            var allPosts = new PostsCollection();
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

        //private async Task<List<PostModel>> GetPostsForMemberFriendAsync(int friendId)
        //{
        //    var allTasks = new List<Task<List<PostModel>>>();
        //    var allPosts = new List<PostModel>();
        //    //var firstHundredPosts = await _api.GetPostsAsync(friendId);
        //    //var count = firstHundredPosts.Count;
        //    //var i = 0;
        //    for (var i = 0; i < 300; i += 100)
        //    {
        //        var hundredPosts = _api.GetPostsAsync(friendId, i);
        //        allTasks.Add(hundredPosts);
        //    }

        //    //while (count > 0)
        //    //{
        //    //    var hundredPosts = _api.GetPostsAsync(friendId, i);
        //    //    count = hundredPosts.Result.Count;
        //    //    allTasks.Add(hundredPosts);
        //    //    i += 100;
        //    //}

        //    var posts = await Task.WhenAll(allTasks).ConfigureAwait(false);

        //    foreach (var postModel in posts)
        //    {
        //        allPosts.AddRange(postModel ?? new List<PostModel>());
        //    }

        //    return allPosts;
        //}

        //public async Task<List<List<PostModel>>> GetAllPostsForUserAsync(int userId)
        //{
        //    var friends = await _api.GetFriendsAsync(userId);
        //    var friendsPostsTasks = new List<Task<List<PostModel>>>();
        //    foreach (var friendsItem in friends.items)
        //    {
        //        friendsPostsTasks.Add(GetPostsForMemberFriendAsync(friendsItem.id));
        //    }

        //    var postsByFriend = await Task.WhenAll(friendsPostsTasks);

        //    return postsByFriend.Where(post => post.Count > 0).ToList();
        //}

        public async Task<PostsCollection> GetAllPostsForUserAsync(int userId)
        {
            var allTasks = new List<Task<PostsCollection>>();
            var friends = await _api.GetFriendsAsync(userId);

            foreach (var friend in friends.items)
            {
                allTasks.Add(GetPostsForMemberFriendAsync(friend.id));
            }

            var results = await Task.WhenAll(allTasks).ConfigureAwait(false);

            var allPosts = new PostsCollection();

            foreach (var post in results)
            {
                allPosts.CopyData(post.items);
                allPosts.count += post.count;
            }
            return allPosts;
        }

        public PostsCollection SortPosts(string filter, PostsCollection allPosts)
        {
            switch (filter)
            {
                case "likes":
                    SortedPosts.items = allPosts.items
                        //.SelectMany(post => post)
                        .OrderByDescending(post => post.likes.count).ToList();
                    break;
                case "comments":
                    SortedPosts.items = allPosts.items
                        //.SelectMany(post => post)
                        .OrderByDescending(post => post.comments.count).ToList();
                    break;
                case "reposts":
                    SortedPosts.items = allPosts.items
                        //.SelectMany(post => post)
                        .OrderByDescending(post => post.reposts.count).ToList();
                    break;
            }
            SortedPosts.count = allPosts.count;
            return SortedPosts;
        }
    }
}