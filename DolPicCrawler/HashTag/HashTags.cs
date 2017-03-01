
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DolPicCrawler.HashTag
{
    public class HashTagData
    {
        [JsonProperty("_id")]
        public string hashTagIndex;

        [JsonProperty("twitterHashTag")]
        public string twitterHashTag;

        [JsonProperty("instagramHashTag")]
        public string instaHashTag;
    }
}
