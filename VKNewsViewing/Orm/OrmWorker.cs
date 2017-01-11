using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using VkClient.Models;

namespace Orm
{
    public class OrmWorker
    {
        public IEnumerable<VkUser> GetFriendsMember(int memberId)
        {
            using (var ctx = new VkContext())
            {
                return ctx.Users
                    .Find(memberId).VkUserTo
                    .Union(ctx.Users.Find(memberId).VkUserFrom)
                    .Where(u => u.Hidden == 0);
            }
        }

        public ICollection<VkUser> GetMembers()
        {
            using (var ctx = new VkContext())
            {
                return ctx.Users.Where(u => u.IsMember).ToList();
            }
        }

        public void AddOrUpdateUser(VkUser user)
        {
            using (var ctx = new VkContext())
            {
                var existingUser = ctx.Users.Find(user.Id);
                if (existingUser == null)
                {
                    ctx.Users.Add(user);
                }
                else
                {
                    user.VkUserTo = existingUser.VkUserTo;
                    user.VkUserFrom = existingUser.VkUserFrom;
                    ctx.Entry(existingUser).CurrentValues.SetValues(user);
                }
                ctx.SaveChanges();
            }
        }

        public VkUser GetUserById(int id)
        {
            using (var ctx = new VkContext())
            {
                return ctx.Users.Find(id);
            }
        }

        public VkPost GetPostById(int ownerId, int postId)
        {
            using (var ctx = new VkContext())
            {
                return ctx.Posts
                    .Include(p => p.VkUserId)
                    .SingleOrDefault(p => p.OwnerId == ownerId && p.PostId == postId);
            }
        }

        public List<VkPost> GetAllPosts()
        {
            using (var ctx = new VkContext())
            {
                return new List<VkPost>(ctx.Posts);
            }
        }

        public List<VkPost> GetMemberFriendsPosts(int memberId)
        {
            using (var ctx = new VkContext())
            {
                var friends = GetFriendsMember(memberId).ToList().Select(f => f.Id);
                var friendPosts = new List<VkPost>();
                foreach (var friend in friends)
                {
                    var posts = ctx.Posts.Include(p => p.VkUserId).Where(p => p.VkUserId.Id == friend).ToList();
                    friendPosts.AddRange(posts);
                }

                var r1 = friendPosts.Where(p => p.VkUserId != null).ToList();
                var allPosts = new HashSet<VkPost>(friendPosts);

                //var reposts = friendPosts.Where(p => p.RepostId != 0).ToList();
                Parallel.ForEach(friendPosts, r =>
                {
                    if (r.RepostId != 0)
                    {
                        allPosts.Add(GetPostById(r.OwnerId, r.PostId));
                    }
                });
                return allPosts.Where(p => p != null).ToList();
            }
        }

        //public List<VkPost> GetPostsForMemberFriends(int memberId)
        //{
        //    using (var ctx = new VkContext())
        //    {
        //        var posts = new List<VkPost>();
        //        var friends = GetFriendsMember(memberId);
        //        foreach (var friend in friends)
        //        {
        //            var postsFriend = ctx.Posts
        //                .Select(p => p)
        //                .Where(p => p.VkUserId.Id == friend.Id)
        //                .ToList();

        //            postsFriend.ForEach(p =>
        //            {
        //                if (p.RepostId != 0)
        //                {
        //                    var post = ctx.Posts.Find(p.FromId, p.RepostId);
        //                    posts.Add(post);
        //                }
        //            });
        //            posts.AddRange(postsFriend);
        //        }
        //        return posts;
        //    }
        //}

        public void RemoveMembers(List<VkUser> users)
        {
            using (var ctx = new VkContext())
            {
                var dbMembers = ctx.Users.Where(m => m.IsMember);
                foreach (var dbMember in dbMembers)
                {
                    var existingUser = users.FirstOrDefault(u => u.Id == dbMember.Id);
                    if (existingUser == null)
                    {
                        var relationsPosts = ctx.Posts.Where(p => p.VkUserId.Id == dbMember.Id).ToList();
                        if (relationsPosts.Count != 0)
                            ctx.Posts.RemoveRange(relationsPosts);

                        dbMember.VkUserTo.Clear();
                        dbMember.VkUserFrom.Clear();
                        ctx.Users.Remove(dbMember);
                    }
                }
                ctx.SaveChanges();
            }
        }

        public void AddOrUpdateFriends(List<int> membersIds, List<IEnumerable<VkClientUserModel>> friends)
        {
            using (var db = new VkContext())
            {
                for (var i = 0; i < membersIds.Count; i++)
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    var groupMember = db.Users.Find(membersIds[i]);
                    foreach (var friend in friends[i])
                    {
                        var existFriend = db.Users.Find(friend.UserId);
                        var newFriend = Mapper.MapVkUserModel(friend);
                        if (existFriend == null)
                        {
                            db.Users.Add(newFriend);
                            if (groupMember.Id > friend.UserId)
                                groupMember.VkUserTo.Add(newFriend);
                            else
                                newFriend.VkUserTo.Add(groupMember);
                        }
                        else
                        {
                            newFriend.VkUserTo = existFriend.VkUserTo;
                            newFriend.VkUserFrom = existFriend.VkUserFrom;
                            db.Entry(existFriend).CurrentValues.SetValues(newFriend);

                            if (groupMember.Id > friend.UserId)
                                groupMember.VkUserTo.Add(existFriend);
                            else
                                existFriend.VkUserTo.Add(groupMember);
                        }
                    }

                    var userFriends = groupMember.VkUserTo.Select(f => f).ToList();
                    foreach (var vkUser in userFriends)
                    {
                        var relation = friends[i].FirstOrDefault(u => u.UserId == vkUser.Id);
                        if (relation == null)
                        {
                            groupMember.VkUserTo.Remove(vkUser);
                        }
                    }

                    userFriends = groupMember.VkUserFrom.Select(f => f).ToList();
                    foreach (var vkUser in userFriends)
                    {
                        var relation = friends[i].FirstOrDefault(u => u.UserId == vkUser.Id);
                        if (relation == null)
                        {
                            groupMember.VkUserFrom.Remove(vkUser);
                        }
                    }

                    db.SaveChanges();
                    db.ChangeTracker.DetectChanges();
                }
            }
        }


        public void AddOrUpdatePosts(object post)
        {
            var vkPost = (VkPost) post;
            using (var ctx = new VkContext())
            {
                var existingPost = ctx.Posts.Find(vkPost.OwnerId, vkPost.PostId);
                vkPost.VkUserId = ctx.Users.Find(vkPost.OwnerId);
                if (existingPost == null)
                    ctx.Posts.Add(vkPost);
                else
                    ctx.Entry(existingPost).CurrentValues.SetValues(vkPost);
                try
                {
                    ctx.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.InnerException as SqlException;
                    if (innerException != null && (innerException.Number == 2627 || innerException.Number == 2601))
                    {
                        return;
                    }
                }
            }
        }
    }
}