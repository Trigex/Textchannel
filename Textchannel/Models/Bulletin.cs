using Newtonsoft.Json;

namespace Textchannel.Models
{
    public class Bulletin
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("poster_username")]
        public string Username { get; set; }
        [JsonProperty("poster_flare")]
        public string Flare { get; set; }
    }

    public class BulletinsRoot
    {
        [JsonProperty("bulletins")]
        public Bulletin[] Bulletins { get; set; }
    }
}
