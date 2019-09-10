using Newtonsoft.Json;

namespace Textchannel.Models
{
    /// <summary>
    /// A Textbin v2 Category object
    /// </summary>
    public class Category
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("rank_required")]
        public int RankRequired { get; set; }
        [JsonProperty("rank_name")]
        public string RankName { get; set; }
    }

    public class CategoriesRoot
    {
        [JsonProperty("categories")]
        public Category[] Categories { get; set; }
    }
}
