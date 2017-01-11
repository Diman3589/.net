using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookApi;
using FbClient.Models;

namespace FbClient
{
    public class Client
    {
        private readonly FbApi _fbApi;

        public Client()
        {
            _fbApi = new FbApi();
        }

        public async Task<List<FbClientPostModel>> GetAllFbPostsInfo(List<string> urls)
        {
            var allTasks = urls.Select(_fbApi.GetFbPostInfo).ToList();
            var result = await Task.WhenAll(allTasks);

            var posts = result.Select(FbMapper.MapPostModel).ToList();
            return posts.Where(p => p != null).ToList();
        }
    }
}