using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Textchannel
{
    public class Cors
    {
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("origin")]
        public string Origin { get; set; }
    }

    public class AppConfiguration
    {
        [JsonProperty("cors")]
        public Cors Cors { get; set; }
    }
}
