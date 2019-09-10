using Newtonsoft.Json;

namespace Textchannel.Models
{
    public class Captcha
    {
        [JsonProperty("base64")]
        public string Base64EncodedImage { get; set; }
    }
}
