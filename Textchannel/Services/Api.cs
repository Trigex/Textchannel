using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TextbinChannel.Services;
using Textchannel.Models;

namespace Textchannel.Services
{
    /// <summary>
    /// Service which makes Api requests to the target chan site, and returns the output
    /// </summary>
    public class Api
    {
        private readonly string _host;
        private readonly string _origin;
        private const string ApiUrl = "https://textbin.termer.net/api/v2";
        private readonly Dictionary<string, string> _errorStringMapping = new Dictionary<string, string>()
        {
            { "banned", "You're banned!" },
            { "invalid_captcha", "The captcha solution was incorrect!" }
        };

        private readonly HttpClient _httpClient;
        private readonly ToastService _toastService;

        public Api(HttpClient client, ToastService toastService, AppConfiguration config)
        {
            _httpClient = client;
            _toastService = toastService;
            _host = config.Cors.Host;
            _origin = config.Cors.Origin;
        }

        #region Category Methods
        /* GET
        */
        public async Task<Category[]> GetCategoriesAsync()
        {
            var categoryResp = await RequestAsync($"{ApiUrl}/categories", "GET", null);
            var categories = JsonConvert.DeserializeObject<CategoriesRoot>(categoryResp.Content);
            return categories.Categories;
        }
        #endregion

        #region Post Methods
        /* GET
         */
        public async Task<Post[]> GetPostsAsync()
        {
            var postResp = await RequestAsync($"{ApiUrl}/public_posts", "GET", null);
            var posts = JsonConvert.DeserializeObject<PostsRoot>(postResp.Content);
            return posts.Posts;
        }

        public async Task<Post[]> GetPostsAsync(string categoryCode)
        {
            var postResp = await RequestAsync($"{ApiUrl}/public_posts?category={categoryCode}", "GET", null);
            var posts = JsonConvert.DeserializeObject<PostsRoot>(postResp.Content);
            return posts.Posts;
        }

        public async Task<Post[]> GetPostsAsync(Category category)
        {
            return await GetPostsAsync(category.Code);
        }

        public async Task<Post> GetPostAsync(string postId)
        {
            var postResp = await RequestAsync($"{ApiUrl}/get_post?id={postId}", "GET", null);
            var post = JsonConvert.DeserializeObject<Post>(postResp.Content);
            return post;
        }

        /* POST
         */
        public async Task<Post> CreatePostAsync(string name, string text, int categoryId)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("category", categoryId.ToString()),
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("type", "plain")
            });

            // send post request
            var resp = await RequestAsync($"{ApiUrl}/post", "POST", content);
            if (resp.StatusString != "error")
                // get thread from the sent back id
                return await GetPostAsync(JsonConvert.DeserializeObject<PostCreated>(resp.Content).PostId);

            return null;
        }
        #endregion

        #region Captcha Methods
        public async Task<Captcha> GetCaptchaAsync()
        {
            var captchaResp = await RequestAsync($"{ApiUrl}/captcha_image", "GET", null);
            return JsonConvert.DeserializeObject<Captcha>(captchaResp.Content);
        }
        #endregion

        #region Comment Methods
        /* GET
        */
        public async Task<Comment[]> GetCommentsAsync(string postId)
        {
            var commentsResp = await RequestAsync($"{ApiUrl}/comments?post_id={postId}", "GET", null);
            var comments = JsonConvert.DeserializeObject<CommentsRoot>(commentsResp.Content);
            return comments.Comments;
        }

        public async Task<Comment[]> GetCommentsAsync(Post post)
        {
            return await GetCommentsAsync(post.Id);
        }

        public async Task<bool> CreateCommentAsync(string captchaSolution, string postId, string comment, string name, string email)
        {
            Console.WriteLine($"{captchaSolution} {postId} {comment} {name} {email}");
            var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("captcha", captchaSolution),
                new KeyValuePair<string, string>("post_id", postId),
                new KeyValuePair<string, string>("comment", comment),
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("email", email)
            });

            var resp = await RequestAsync($"{ApiUrl}/post_reply", "POST", content);
            Console.WriteLine(resp.ErrorString);
            if (resp.StatusString == "success")
                return true;
            else
                return false;
        }
        #endregion

        #region Bulletin Methods
        public async Task<Bulletin[]> GetBulletinsAsync()
        {
            var bulletinResp = await RequestAsync($"{ApiUrl}/bulletins", "GET", null);
            var bulletins = JsonConvert.DeserializeObject<BulletinsRoot>(bulletinResp.Content);
            return bulletins.Bulletins;
        }
        #endregion

        #region Internal Http Request Execution Methods
        private async Task<ApiResponse> RequestAsync(string uri, string method, HttpContent content)
        {
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(uri),
                Content = content
            };
            // sets proper cors headers
            SetCorsHeaders(request);
            // get server response
            var response = await _httpClient.SendAsync(request);

            // string response body
            var responseBody = await response.Content.ReadAsStringAsync();
            // convert to ApiResponse object through newtonsoft, which will set the status and error properties
            var apiResp = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
            // set the httpstatus code of the ApiResponse to the http response
            apiResp.StatusCode = response.StatusCode;
            apiResp.Content = responseBody;

            Console.WriteLine(apiResp.StatusString);
            if (apiResp.StatusString == "error")
            {
                string errMessage;
                // get full error message mapping
                _errorStringMapping.TryGetValue(apiResp.ErrorString, out errMessage);
                if (errMessage == null)
                    errMessage = apiResp.ErrorString;

                _toastService.ShowToast(errMessage, ToastLevel.Error);
            }
            return apiResp;
        }

        private void SetCorsHeaders(HttpRequestMessage req)
        {
            req.Headers.Add("Host", _host);
            req.Headers.Add("Origin", _origin);
            req.Headers.Add("Connection", "keep-alive");
            req.Headers.Add("Accept", "*/*");
        }
        #endregion
    }

    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        [JsonProperty("status")]
        public string StatusString { get; set; }
        [JsonProperty("error")]
        public string ErrorString { get; set; }
        public string Content { get; set; }
    }
}
