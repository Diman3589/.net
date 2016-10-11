using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class HomeController : Controller
    {
        private Graph _graph;
        private UsersCollection _members;
        private Dictionary<int, List<UserModel>> _allFriends;
        private VkApi _vkApi;
        private double _time;

        public HomeController()
        {
            _graph = new Graph();
            _vkApi = new VkApi();
        }

        public async Task<ActionResult> Posts(int userId)
        {
            var sw = Stopwatch.StartNew();
            var allPosts = await _graph.GetAllPostsForUserAsync(userId);//_members.items[0].id);

            var sortedPosts = _graph.SortPosts("likes", allPosts);
            ViewBag.Message = sw.Elapsed.TotalMilliseconds;

            return View(sortedPosts);
        }

        public async Task<ViewResult> Index()
        {
            _members = await _graph.GetMembersAsync(30390813);
            var sw = Stopwatch.StartNew();
            _allFriends = await _graph.GetAllFriendsAsync(_members.items);
            _time = sw.Elapsed.TotalMilliseconds;
            ViewBag.Message = _time;

            return View(_members);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            
            return View();
        }

    }
}