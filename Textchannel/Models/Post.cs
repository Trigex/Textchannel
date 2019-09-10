using Newtonsoft.Json;

namespace Textchannel.Models
{
    /// <summary>
    /// A Textbin v2 Post object
    /// </summary>
    public class Post
    {
        [JsonProperty("post_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("category")]
        public int CategoryId { get; set; }
        [JsonProperty("category_code")]
        public string CategoryCode { get; set; }
        [JsonProperty("category_name")]
        public string CategoryName { get; set; }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty("sticky")]
        public bool IsSticky { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class PostsRoot
    {
        [JsonProperty("posts")]
        public Post[] Posts { get; set; }
    }

    public class PostCreated
    {
        [JsonProperty("post_id")]
        public string PostId { get; set; }
    }
}
