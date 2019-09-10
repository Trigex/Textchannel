using Newtonsoft.Json;

namespace Textchannel.Models
{
    /// <summary>
    /// A Textbin v2 Comment object
    /// </summary>
    public class Comment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("post_id")]
        public string PostId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("poster_rank")]
        public int PosterRank { get; set; }
        [JsonProperty("ban_text")]
        public string BanText { get; set; }
        [JsonProperty("category")]
        public int CategoryId { get; set; }
        [JsonProperty("category_code")]
        public string CategoryCode { get; set; }
        [JsonProperty("rank_name")]
        public string RankName { get; set; }
        [JsonProperty("rank_flare")]
        public string RankFlare { get; set; }
        [JsonProperty("you")]
        public bool You { get; set; }

        /// <summary>
        /// Utility method to turn a Post into a displayable comment,
        /// making posts more in-line with a typical chan
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static Comment PostToComment(Post post)
        {
            return new Comment
            {
                Name = "OP",
                Text = post.Text,
                Date = post.Date,
                Time = post.Time,
                CategoryId = post.CategoryId,
                CategoryCode = post.CategoryCode
            };
        }
    }

    public class CommentsRoot
    {
        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }
    }
}
