using DolPicCrawler.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DolPicCrawler.HashTag
{
    public class InstagramHashTag : OriginHashTag
    {
        private const string CON_IMAGE_URL = "https://instagram.com/{0}";
        private const string CON_IMAGE_MATCH_TAG = "\"display_src\": \"(?<" + CommonConst.IMAGE_MATCH_STRING + ">.*?)\".*?";
        private const string CON_CAPTION_MATCH_TAG = "\"caption\": \"(?<" + CommonConst.CAPTION_MATCH_STRING + ">.*?)\".*?";

        /// <summary>
        /// 해당 사이트에서 이미지 경로 추출
        /// </summary>
        /// <param name="a_listHashTag">해쉬태그</param>
        /// <param name="a_dImage">이미지 정보 Dictionary</param>
        /// <param name="a_dCaption">내용 정보 Dictionary</param>
        public override void ImageSrcSearch(
                List<HashTagData> a_listHashTag,
                ref Dictionary<string, HashTagQueryData> a_dImage,
                ref Dictionary<string, HashTagQueryData> a_dCaption
            )
        {
            var list = a_listHashTag.Where(c => c.instaHashTag != "");

            // 해쉬 태그대로 검색
            foreach (var tag in list)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(string.Format(CON_IMAGE_URL, tag.instaHashTag)).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    // 결과물에서 이미지 URL 추출
                    ImageSearch(content, tag.hashTagIndex, ref a_dImage, ref a_dCaption, CON_IMAGE_MATCH_TAG, CON_CAPTION_MATCH_TAG, false);
                }
            }
        }
    }
}
