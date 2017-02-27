using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DolPicCrawler.HashTag
{
    public class InstagramHashTag : OriginHashTag
    {
        private const string CON_IMAGE_URL = "https://instagram.com/{0}";
        private const string CON_MATCH_TAG = "\"display_src\": \"(?<ImageSrc>.*?)\".*?";

        /// <summary>
        /// 해당 사이트에서 이미지 경로 추출
        /// </summary>
        /// <param name="a_listHashTag">해쉬태그</param>
        /// <param name="a_dImage">이미지 정보 Dictionary</param>
        public override void ImageSrcSearch(List<HashTagData> a_listHashTag, ref Dictionary<string, List<string>> a_dImage)
        {
            var list = a_listHashTag.Where(c => c.instaHashTag != "");

            // 해쉬 태그대로 검색
            foreach (var tag in list)
            {
                var content = "";

                using (var client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(CON_IMAGE_URL, tag.instaHashTag));
                    var task = client.SendAsync(request).ContinueWith(responseTask =>
                    {
                        content = responseTask.Result.Content.ReadAsStringAsync().Result;
                    });

                    task.Wait();

                    // 결과물에서 이미지 URL 추출
                    ImageSearch(content, tag.hashTagIndex, ref a_dImage, CON_MATCH_TAG, true);
                }
            }
        }
    }
}
