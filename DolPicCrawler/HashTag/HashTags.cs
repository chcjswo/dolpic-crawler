
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DolPicCrawler.HashTag
{
    public class HashTags
    {
        [JsonProperty("_id")]
        public string hashTagIndex;
        [JsonProperty("twitterHashTag")]
        public string twitterHashTag;
        [JsonProperty("instaHashTag")]
        public string instaHashTag;
    }

    [JsonArray]
    public class Jobs
    {
        public List<Job> JSON;
    }

    public class Job
    {
        [JsonProperty("organization_name")]
        public string Organization { get; set; }
        [JsonProperty("position_title")]
        public string Title { get; set; }
    }

}
