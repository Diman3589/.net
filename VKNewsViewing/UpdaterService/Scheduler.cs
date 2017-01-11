using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UpdaterService
{
    public abstract class Scheduler
    {
        public abstract Task Run();

        public static void Main(string[] args)
        {
            var usersUpdater = new UsersUpdater();
            var postsUpdater = new PostsUpdater();
            var schedulers = new List<Scheduler>
            {
                usersUpdater,
                postsUpdater
            };

            postsUpdater.Run().Wait();

            //while (true)
            //{
            //    var timer = new Stopwatch();

            //    foreach (var scheduler in schedulers)
            //    {
            //        timer.Start();
            //        scheduler.Run().Wait();
            //        timer.Stop();
            //        Console.WriteLine(timer.ElapsedMilliseconds);
            //        timer.Reset();

            //    }
            //    Thread.Sleep(2160000);
            //}
        }
    }
}