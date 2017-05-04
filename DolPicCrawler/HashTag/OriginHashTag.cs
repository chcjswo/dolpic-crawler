using DolPicCrawler.Utils;
using System;
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
        public abstract void ImageSrcSearch(
                List<HashTagData> a_listHashTag,
                ref Dictionary<string, HashTagQueryData> a_dicHashTagData
            );

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
        /// <param name="a_sTagIndex">해쉬태그 고유번호</param>
        /// <param name="a_dicHashTagData"></param>
        /// <param name="a_image_tag"></param>
        /// <param name="a_caption_tag"></param>
        /// <param name="isReplace"></param>
        public void ImageSearch(
                string resString,
                string a_sTagIndex,
                ref Dictionary<string, HashTagQueryData> a_dicHashTagData,
                string a_image_tag,
                string a_caption_tag,
                bool isReplace
            )
        {
            Console.WriteLine("index === " + a_sTagIndex);
            try
            {
                // Dictionary 저장
                a_dicHashTagData.Add(
                    a_sTagIndex,
                    GetMatchString(
                        resString,
                        isReplace,
                        a_image_tag,
                        CommonConst.IMAGE_MATCH_STRING,
                        a_caption_tag,
                        CommonConst.CAPTION_MATCH_STRING
                    )
                );
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        
        }

        private HashTagQueryData GetMatchString(
                string resString,
                bool isReplace,
                string a_image_tag,
                string a_image_match_string,
                string a_caption_tag,
                string a_caption_match_string
            )
        {
            HashTagQueryData hashTagQueryData = new HashTagQueryData();

            // 이미지 찾기
            Regex re = new Regex(a_image_tag, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            hashTagQueryData.listImageSrc = new List<string>();

            for (Match m = re.Match(resString); m.Success; m = m.NextMatch())
            {
                var sImageSrc = m.Groups[a_image_match_string].Value;
                if (isReplace)
                    hashTagQueryData.listImageSrc.Add(sImageSrc.Replace("\\", ""));
                else
                    hashTagQueryData.listImageSrc.Add(sImageSrc);
            }

            re = new Regex(a_caption_tag, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            hashTagQueryData.listCaptionString = new List<string>();

            for (Match m = re.Match(resString); m.Success; m = m.NextMatch())
            {
                var sImageSrc = m.Groups[a_caption_match_string].Value;
                hashTagQueryData.listCaptionString.Add(sImageSrc);
            }

            return hashTagQueryData;
        }
    }
}
