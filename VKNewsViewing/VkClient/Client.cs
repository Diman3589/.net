using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using VkClient.Models;
using VkontakteApi;
using VkontakteApi.Models;

namespace VkClient
{
    public class Client
    {
        private async Task<List<UserModel>> GetFriendsForMemberAsync(int userId)
        {
            var userFriends = await VkApi.GetFriendsAsync(userId);
            var editedFriends = userFriends.items
                .Select(userFriend => VkApi.GetCountFriendsAsync(userFriend.id))
                .ToList();

            var resultFriends = await Task.WhenAll(editedFriends);

            for (var i = 0; i < resultFriends.Length; i++)
            {
                userFriends.items[i].friends_count = resultFriends[i];
            }
            return userFriends.items;
        }

        public async Task<List<IEnumerable<VkClientUserModel>>> GetAllFriends(IEnumerable<int> memberIds)
        {
            var tasks = memberIds.Select(GetFriendsForMemberAsync).ToList();

            var res = await Task.WhenAll(tasks).ConfigureAwait(false);

            return res
                .Select(userModel => userModel.Select(Mapper.MapUserModel).ToList())
                .Cast<IEnumerable<VkClientUserModel>>()
                .ToList();
        }

        public async Task<List<VkClientUserModel>> GetMembersAsync(int groupId)
        {
            var members = await VkApi.GetMembersAsync(groupId);
            var editedMembers = members.items.Select(member => VkApi.GetCountFriendsAsync(member.id)).ToList();

            var resultMembers = await Task.WhenAll(editedMembers);
            for (var i = 0; i < resultMembers.Length; i++)
            {
                var resultMember = resultMembers[i];
                members.items[i].friends_count = resultMember;
                members.items[i].IsMember = true;
            }
            return members.items.Select(Mapper.MapUserModel).ToList();
        }

        private async Task<PostsCollection> GetPostsForMemberFriendAsync(int friendId)
        {
            //var allPosts = new PostsCollection();
            var hundredPosts = await VkApi.GetPostsAsync(friendId, 100);

            //var editedPosts = new List<Task<Dictionary<string, int>>>();
            //foreach (var postModel in hundredPosts.items)
            //{
            //    if (postModel.owner_id <= 0) continue;
            //    var post = VkApi.GetPostInfoAsync(postModel.owner_id, postModel.id);
            //    editedPosts.Add(post);
            //}
            //var resultPosts = await Task.WhenAll(editedPosts);
            //for (var i = 0; i < resultPosts.Length; i++)
            //{
            //    hundredPosts.items[i].comments.count = resultPosts[i]["comments"];
            //    hundredPosts.items[i].likes.count = resultPosts[i]["likes"];
            //    hundredPosts.items[i].reposts.count = resultPosts[i]["reposts"];
            //}
            //while (hundredPosts != null && hundredPosts.items.Count != 0)
            //{
            //allPosts.CopyData(hundredPosts.items);
            //allPosts.count = hundredPosts.count;
            //hundredPosts = await _api.GetPostsAsync(friendId);
            //}
            return hundredPosts;
            //return allPosts;
        }

        public async Task<List<List<VkClientPostModel>>> GetAllPostsMemberFriendsAsync(IEnumerable<int> userIds)
        {
            var allTasks = userIds.Select(GetPostsForMemberFriendAsync).ToList();

            var result = await Task.WhenAll(allTasks);

            var allPosts = new PostsCollection();

            foreach (var post in result)
            {
                allPosts.items.AddRange(post.items);
            }

            var resPosts = allPosts.items.Select(Mapper.MapPostModel).ToList();
            return resPosts.Where(p => p != null).ToList();
            //var ownerInfoTasks = (from resPost in resPosts
            //                      from post in resPost
            //                      select GetInfoAboutPostOwner(post.OwnerId)).ToList();

            //var ownerInfoRes = await Task.WhenAll(ownerInfoTasks);

            //resPosts.ForEach(post => post
            //    .ForEach(p =>
            //        {
            //            var ownInfo = ownerInfoRes.FirstOrDefault(o => o[0] == p.OwnerId.ToString());
            //            if (ownInfo == null) return;
            //            p.PostOwnerName = ownInfo[1];
            //            p.PostOwnerPhoto = ownInfo[2];
            //        }
            //    ));

        }

        private async Task<string[]> GetInfoAboutPostOwner(int id)
        {
            if (id > 0)
            {
                var userInfo = await VkApi.GetUserInfoAsync(id);
                return new[] {id.ToString(), userInfo[0], userInfo[1]};
            }
            var groupInfo = await VkApi.GetGroupInfoAsync(id);
            return new[] {id.ToString(), groupInfo[0], groupInfo[1]};
        }

        //public PostsCollection SortPosts(string filter, PostsCollection allPosts)
        //{
        //    switch (filter)
        //    {
        //        case "likes":
        //            SortedPosts.items = allPosts.items
        //                //.SelectMany(post => post)
        //                .OrderByDescending(post => post.likes.count).ToList();
        //            break;
        //        case "comments":
        //            SortedPosts.items = allPosts.items
        //                //.SelectMany(post => post)
        //                .OrderByDescending(post => post.comments.count).ToList();
        //            break;
        //        case "reposts":
        //            SortedPosts.items = allPosts.items
        //                //.SelectMany(post => post)
        //                .OrderByDescending(post => post.reposts.count).ToList();
        //            break;
        //    }
        //    SortedPosts.count = allPosts.count;
        //    return SortedPosts;
        //}
    }
}