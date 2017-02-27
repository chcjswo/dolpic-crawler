using System.Collections.Generic;
using System.Xml;

namespace DolPicCrawler.HashTag
{
    public class FacebookHashTag : OriginHashTag
    {
        /// <summary>
        /// 해당 사이트에서 이미지 경로 추출
        /// </summary>
        /// <param name="a_listHashTag">해쉬태그 리스트</param>
        /// <param name="a_dImage">정보를 담을 Dictionary</param>
        public override void ImageSrcSearch(List<HashTagData> a_listHashTag, ref Dictionary<string, List<string>> a_dImage)
        {
        }
    }
}
