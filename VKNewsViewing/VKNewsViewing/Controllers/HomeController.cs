using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class HomeController : Controller
    {
        private readonly Graph _graph;
        private UsersCollection _members;
        private double _time;

        public HomeController()
        {
            _graph = new Graph();
        }

        public async Task<ActionResult> Posts(int userId)
        {
            var sw = Stopwatch.StartNew();
            var allPosts = await _graph.GetAllPostsForUserAsync(userId);

            var sortedPosts = _graph.SortPosts("likes", allPosts);
            ViewBag.Message = sw.Elapsed.TotalMilliseconds;

            return View(sortedPosts);
        }

        public async Task<ViewResult> Index()
        {
            //var result = await _vkApi.GetPostsAsync(73812540);
            _members = await _graph.GetMembersAsync(30390813);
            var sw = Stopwatch.StartNew();
            await _graph.GetAllFriendsAsync(_members.items);
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