using System.Collections.Generic;
using System.Net.Http;

namespace DolPicCrawler.HashTag
{
    public class TwitterHashTag : OriginHashTag
    {
        private const string CON_IMAGE_URL = "https://twitter.com/hashtag/{0}?f=images&vertical=default";
        private const string CON_IMAGE_MATCH_TAG = "data-url=\"(?<ImageSrc>.*?)\".*?";
        private const string CON_CAPTION_MATCH_TAG = "<div class=\"js-tweet-text-container\">(?<ImageSrc>.*?)\".*?</div>";

        /// <summary>
        /// 해당 사이트에서 이미지 경로 추출
        /// </summary>
        /// <param name="a_listHashTag">해쉬태그 고유번호</param>
        /// <param name="a_listTwitterHashTag">해쉬태그</param>
        /// <param name="a_dImage">이미지 정보 Dictionary</param>
        /// <param name="a_dCaption">내용 정보 Dictionary</param>
        public override void ImageSrcSearch(
                List<HashTagData> a_listHashTag,
                ref Dictionary<string, HashTagQueryData> a_dicHashTagData
            )
        {
            // 해쉬 태그대로 검색
            foreach (var tag in a_listHashTag)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(string.Format(CON_IMAGE_URL, tag.twitterHashTag)).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    // 결과물에서 이미지 URL 추출
                    ImageSearch(content, tag.hashTagIndex, ref a_dicHashTagData, CON_IMAGE_MATCH_TAG, CON_CAPTION_MATCH_TAG, false);
                }
            }
        }
    }
}
