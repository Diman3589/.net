using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Orm;
using PostsHandler;
using VkClient;

namespace UpdaterService
{
    public class PostsUpdater : Scheduler
    {
        private readonly OrmWorker _orm;
        private readonly Client _vkClient;
        private readonly VkPostsPreparer _postsPreparer;

        public PostsUpdater()
        {
            _vkClient = new Client();
            _orm = new OrmWorker();
            _postsPreparer = new VkPostsPreparer();
        }

        public override async Task Run()
        {
            await UpdatePosts();
        }

        private async Task UpdatePosts()
        {
            var members = _orm.GetMembers();
            foreach (var member in members)
            {
                var friendsMember = _orm.GetFriendsMember(member.Id).ToList();
                var friendsIds = new HashSet<int>();
                friendsMember.ForEach(f => friendsIds.Add(f.Id));
                var posts = await GetPostsFromVk(friendsIds);

                var threads = new List<Thread>();
                for (var i = 0; i < posts.Count; i++)
                {
                    threads.Add(new Thread(_orm.AddOrUpdatePosts));
                }
                for (var i = 0; i < posts.Count; i++)
                {
                    threads[i].Start(posts[i]);
                    //Thread.Sleep(5);
                }
            }
        }

        private async Task<List<VkPost>> GetPostsFromVk(IEnumerable<int> userIds)
        {
            var res = await _vkClient.GetAllPostsMemberFriendsAsync(userIds);
            var allPosts = res.SelectMany(p => p).ToList();

            return await _postsPreparer.PreparePosts(allPosts);
        }

        private IEnumerable<int> GetAllFriends()
        {
            var members = _orm.GetMembers();
            var allFriends = new HashSet<int>();

            foreach (var member in members)
            {
                var friendsMember = _orm.GetFriendsMember(member.Id);
                friendsMember.ToList().ForEach(f => allFriends.Add(f.Id));
            }

            return allFriends;
        }
    }
}