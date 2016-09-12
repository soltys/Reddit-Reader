using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace RedditReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Initialized += OnInitialized;
            InitializeComponent();
        }

        private async void OnInitialized(object sender, EventArgs e)
        {

            var access_token = await GetRedditAccessToken();


            var redditService = new RedditService(access_token);
            DataContext = new RedditReaderViewModel(redditService);

        }

        #region TOP SECRET
        private static async Task<string> GetRedditAccessToken()
        {
            HttpClient client = new HttpClient();
            var user = ApplicationSettingsService.RedditUserName;
            var password = ApplicationSettingsService.RedditPassword;
            var appId = ApplicationSettingsService.AppId;
            var appSecret = ApplicationSettingsService.AppSecret;
            var url = "https://www.reddit.com/api/v1/access_token";

            var values = new Dictionary<string, string>();
            values["grant_type"] = "password";
            values["username"] = user;
            values["password"] = password;

            var content = new FormUrlEncodedContent(values);


            var byteArray = Encoding.ASCII.GetBytes($"{appId}:{appSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Add("User-Agent", "DGS RedditReader");
            var result = await client.PostAsync(url, content);

            var json = await result.Content.ReadAsStringAsync();
            var access_token = JObject.Parse(json)["access_token"].ToObject<string>();
            return access_token;
        }
        #endregion TOP SECRET

    }
}
