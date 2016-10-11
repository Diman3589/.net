using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using VKNewsViewing.Models;

namespace VKNewsViewing.Controllers
{
    public class HomeController : Controller
    {
        private Graph _graph;
        private UsersCollection _members;
        private Dictionary<int, Task<List<UserModel>>> _allFriends;
        private double _time;

        public HomeController()
        {

        }
        public ActionResult Users()
        {
            WebRequest req = WebRequest.Create("https://oauth.vk.com/authorize?" +
                                                      "client_id=5652000&" +
                                                      "scope=friends,wall,groups&" +
                                                      "redirect_uri=https://oauth.vk.com/blank.html&" +
                                                      "response_type=token" +
                                                      "v=5.56");
            WebResponse resp1 = req.GetResponse();
            StreamReader sr = new StreamReader(resp1.GetResponseStream());
            string jsonObj = sr.ReadToEnd();

            return null;
        }

        public async Task<ViewResult> Index()
        {
            var api = new VkApi();
            _graph = new Graph();
            _members = await _graph.GetMembersAsync(30390813);

            var res = await api.GetPostsAsync(13221962, 0);
            var sw = Stopwatch.StartNew();
            _allFriends = await _graph.GetAllFriendsAsync(_members.items);
            _time = sw.Elapsed.TotalMilliseconds;
            ViewBag.Message = _time;

            sw = Stopwatch.StartNew();
            await _graph.GetAllPostsForUserAsync(24049528);
            var posts = _graph.SortedPosts;
            _time = sw.Elapsed.TotalMilliseconds;

            return View(_allFriends);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}