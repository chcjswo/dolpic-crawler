
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DolPicCrawler.HashTag
{
    public class HashTagData
    {
        /// <summary>
        /// 해쉬태그 고유번호
        /// </summary>
        [JsonProperty("_id")]
        public string hashTagIndex;

        /// <summary>
        /// 트위터 해쉬태그
        /// </summary>
        [JsonProperty("twitterHashTag")]
        public string twitterHashTag;

        /// <summary>
        /// 인스타그램 해쉬태그
        /// </summary>
        [JsonProperty("instagramHashTag")]
        public string instaHashTag;
    }

    public class HashTagQueryData
    {
        /// <summary>
        /// 이미지 url이 저장되는 리스트
        /// </summary>
        public List<string> listImageSrc;

        /// <summary>
        /// 이미지 내용이 저장되는 리스트
        /// </summary>
        public List<string> listCaptionString;
    }
}
