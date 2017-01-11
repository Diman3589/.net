using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using VkClient;
using Orm;
using Mapper = Orm.Mapper;

namespace UpdaterService
{
    public class UsersUpdater : Scheduler
    {
        private readonly Client _vkClient;
        private readonly OrmWorker _orm;
        private const int GroupId = 30390813;//30390813;57770150;137043056

        public UsersUpdater()
        {
            _vkClient = new Client();
            _orm = new OrmWorker();
        }
        public override async Task Run()
        {
            await UpdateUsers();
        }

        private async Task UpdateUsers()
        {
            var members = await GetMembers();
            members.ForEach(m => _orm.AddOrUpdateUser(m));

            var memberIds = members.Select(m => m.Id).ToList();
            var timer = new Stopwatch();
            timer.Start();

            var graphFriends = await _vkClient.GetAllFriends(memberIds);

            timer.Stop();

            Console.WriteLine(timer.ElapsedMilliseconds);

            _orm.AddOrUpdateFriends(memberIds, graphFriends);
            _orm.RemoveMembers(members);
        }

        private async Task<List<VkUser>> GetMembers()
        {
            var members = await _vkClient.GetMembersAsync(GroupId);
            return members.Select(Mapper.MapVkUserModel).ToList();
        }

    }
}
