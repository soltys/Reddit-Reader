using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RedditReader
{
    public class RedditService
    {
        private string _accessToken;
        private HttpClient _client;

        public RedditService(string accessToken)
        {
            this._accessToken = accessToken;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            _client.DefaultRequestHeaders.Add("User-Agent", "DGS RedditReader");
        }

        public async Task<IEnumerable<RedditLink>> GetSubRedditLinksAsync(string subReddit)
        {

      
            var url = $"https://oauth.reddit.com/r/{subReddit}/top.json";
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var responseObject = JObject.Parse(json);
            var children = responseObject["data"]["children"].Value<JArray>();

            var redditLinks = new List<RedditLink>();
            foreach (var entry in children)
            {
                
                redditLinks.Add(new RedditLink
                {
                    Id = entry["data"]["id"].ToObject<string>(),
                    Title = entry["data"]["title"].ToObject<string>(),
                    Score = entry["data"]["score"].ToObject<int>(),
                    SubReddit = subReddit
                });

            }
            return redditLinks;


        }

        public async Task<IEnumerable<RedditComment>> GetRedditCommentsAsync(RedditLink redditLink)
        {
            var url = $"https://oauth.reddit.com/r/{redditLink.SubReddit}/comments/{redditLink.Id}.json?depth=1";

            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var responseObject = JArray.Parse(json);
            var redditComments = new List<RedditComment>();
            foreach (var token in responseObject)
            {
                var commentObject = token.Value<JObject>();
                var children = commentObject["data"]["children"].Value<JArray>();
                foreach (var entry in children)
                {
                    if (entry["kind"].ToObject<string>() == "t1")
                    {
                        redditComments.Add(new RedditComment
                        {
                            Comment = entry["data"]["body"].ToObject<string>(),
                            UserName = entry["data"]["author"].ToObject<string>(),
                        });
                    }
                    
                }

            }

            return redditComments;

        }
    }
}