using System;
using FacebookApi.Models;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FacebookApi
{
    public class FbApi
    {
        private static async Task<string> MakeRequestAsync(string url)
        {
            var req = WebRequest.Create($"https://graph.facebook.com?id={url}");
            try
            {
                using (var resp = await req.GetResponseAsync())
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var json = sr.ReadToEnd();
                        return json;
                    }
                }
            }
            catch (WebException e)
            {
                return null;
            }
        }

        public async Task<FbPostModel> GetFbPostInfo(string url)
        {
            var json = await MakeRequestAsync(url);
            return json == null ? null : JsonConvert.DeserializeObject<FbPostModel>(json);
        }
    }
}