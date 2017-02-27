using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DolPicCrawler.HashTag
{
    /// <summary>
    /// 클래스 enum 타입
    /// </summary>
    public enum OriginSiteType
    {
        twitter = 1,
        instagram,
        facebook
    }

    public abstract class OriginHashTag
    {
        /// <summary>
        /// 해당 사이트에서 이미지 경로 추출
        /// </summary>
        /// <param name="listHashTag">해쉬태그 리스트</param>
        /// <param name="a_dImage">이미지 정보 Dictionary</param>
        public abstract void ImageSrcSearch(List<HashTagData> listHashTag, ref Dictionary<string, List<string>> a_dImage);

        /// <summary>
        /// 객체 생성 팩토리 클래스
        /// </summary>
        /// <param name="siteType">생성 클래스 타입</param>
        /// <returns></returns>
        public static OriginHashTag JsonFactory(OriginSiteType siteType)
        {
            switch (siteType)
            {
                case OriginSiteType.twitter:
                    return new TwitterHashTag();

                case OriginSiteType.instagram:
                    return new InstagramHashTag();

                case OriginSiteType.facebook:
                    return new FacebookHashTag();
            }

            throw new System.NotSupportedException("The OriginSiteType " + siteType.ToString() + " is not recognized.");
        }

        /// <summary>
        /// 이미지 찾기
        /// </summary>
        /// <param name="resString">결과 스트링</param>
        public void ImageSearch(string resString, string a_sTagIndex, ref Dictionary<string, List<string>> a_dImage, string a_match_tag, bool isReplace)
        {
            // 이미지 찾기
            Regex re = new Regex(a_match_tag, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            List<string> ltImg = new List<string>();

            for (Match m = re.Match(resString); m.Success; m = m.NextMatch())
            {
                string sImageSrc = m.Groups["ImageSrc"].Value;
                if (isReplace)
                    ltImg.Add(sImageSrc.Replace("\\", ""));
                else
                    ltImg.Add(sImageSrc);
            }

            // Dictionary 저장
            a_dImage.Add(a_sTagIndex, ltImg);
        }
    }
}
