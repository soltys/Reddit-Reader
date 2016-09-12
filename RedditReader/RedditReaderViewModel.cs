using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ReactiveUI;
using RedditReader.Annotations;

namespace RedditReader
{
    public class RedditReaderViewModel : ReactiveObject,IDisposable
    {
        public  ReactiveList<string> SubReddits { get; private set; }
        public  ReactiveList<RedditLink> RedditLinks { get; private set; }
        public  ReactiveList<RedditComment> RedditComments { get; private set; }

        List<IDisposable> disposables = new List<IDisposable>();
        private RedditService _redditService;
        
        public RelayCommand<string> NewSubRedditCommand { get; private set; }
        public RelayCommand<object> AddManySubRedditsCommand { get; private set; }


        private int _redditLinksCount;
        public int RedditLinksCount
        {
            get { return _redditLinksCount; }
            set { this.RaiseAndSetIfChanged(ref _redditLinksCount, value); }
        }

        private int _redditCommentsCount;
        public int RedditCommentsCount
        {
            get { return _redditCommentsCount; }
            set { this.RaiseAndSetIfChanged(ref _redditCommentsCount, value); }
        }

        public RedditReaderViewModel(RedditService redditService)
        {
            this._redditService = redditService;
            
            NewSubRedditCommand = new RelayCommand<string>((newSubRedditName) =>
            {
               SubReddits.Add(newSubRedditName); 
            });

            AddManySubRedditsCommand = new RelayCommand<object>((_) =>
            {
                for (int i =0;i<5;i++)
                {
                    SubReddits.Add("programming");
                    SubReddits.Add("games");
                    SubReddits.Add("funny");
                    SubReddits.Add("pics");
                    SubReddits.Add("me_irl");
                }
            });

            SubReddits = new ReactiveList<string>();
            RedditLinks = new ReactiveList<RedditLink>();
            RedditComments = new ReactiveList<RedditComment>();

            var onSubRedditAdded = SubReddits.ItemsAdded.Subscribe(async subReddit =>
            {
                var subLinks = await _redditService.GetSubRedditLinksAsync(subReddit);
                foreach (var redditLink in subLinks)
                {
                    await Task.Delay(2000);
                    RedditLinks.Add(redditLink);
                    RedditLinksCount = RedditLinks.Count;
                }
            });
            disposables.Add(onSubRedditAdded);

            var onLinkAdded = RedditLinks.ItemsAdded.Subscribe(async redditLink =>
            {
                var comments = await _redditService.GetRedditCommentsAsync(redditLink);
                foreach (var comment in comments)
                {
                    RedditComments.Add(comment);
                    RedditCommentsCount = RedditComments.Count;
                }
            });
            disposables.Add(onLinkAdded);
        }


        public void Dispose()
        {
            
        }

        

      
    }

    public class RedditLink
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public string SubReddit { get; set; }
    }

    public class RedditComment
    {
        public string UserName { get; set; }
        public string Comment { get; set; }
    }
}