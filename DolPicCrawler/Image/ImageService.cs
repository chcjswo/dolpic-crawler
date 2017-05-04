using DolPicCrawler.HashTag;
using DolPicCrawler.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace DolPicCrawler.Image
{
    public class ImageService
    {
        private static readonly ImageService _imageService;
        private readonly string image_insert_url = ConfigurationManager.AppSettings["dolpicImageInsertUrl"].ToString();
        private readonly string slack_notify_url = ConfigurationManager.AppSettings["slackNotifyUrl"].ToString();

        /// <summary>
        /// static 초기화
        /// </summary>
        static ImageService()
        {
            _imageService = new ImageService();
        }

        public static ImageService getInstance
        {
            get
            {
                return _imageService;
            }
        }

        public object CommonUtil { get; private set; }

        #region 이미지 저장하기
        /// <summary>
        /// 이미지 저장하기
        /// </summary>
        /// <param name="a_dicHashTagData">저장할 이미지 정보/이미지 내용 Dictionary</param>
        /// <param name="a_nTagUrlType">사이트 타입 1:트위터 2:인스타그램 3:페이스북</param>
        public async void ImageSend(
                Dictionary<string, HashTagQueryData> a_dicHashTagData,
                int a_nTagUrlType
            )
        {
            using (var client = new HttpClient())
            {
                foreach (KeyValuePair<string, HashTagQueryData> kvp in a_dicHashTagData)
                {
                    var captionCount = kvp.Value.listCaptionString.Count;

                    for (var i=0; i<kvp.Value.listImageSrc.Count; i++)
                    {
                        // 이미지 Bsee64 인코딩
                        var imageBase64 = CommonUtils.getStringToBase64(kvp.Value.listImageSrc[i]);
                        var values = new Dictionary<string, string>();
                        values.Add("hashTagId", kvp.Key);
                        values.Add("urlType", a_nTagUrlType.ToString());
                        values.Add("imageUrl", imageBase64);

                        Console.WriteLine("url == " + string.Format(image_insert_url, kvp.Key, imageBase64, a_nTagUrlType, 1));
                        Console.WriteLine("TagNo == " + kvp.Key);
                        Console.WriteLine("ImageSrc == " + imageBase64);

                        if (captionCount > i)
                        {
                            var caption = CommonUtils.DecodeEncodedNonAsciiCharacters(
                                CommonUtils.DecodeEncodedNonAsciiCharacters(kvp.Value.listCaptionString[i])
                            );

                            var captionBase64 = CommonUtils.getStringToBase64(caption);
                            values.Add("caption", captionBase64);

                            Console.WriteLine("caption base64 == " + captionBase64);
                        }

                        var content = new FormUrlEncodedContent(values);
                        await client.PostAsync(image_insert_url, content);

                        //Console.WriteLine("result == " + result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }
        #endregion

        #region 슬랙 메시지 보내기 호출
        /// <summary>
        /// 이미지 저장하기
        /// </summary>
        public void SlackNotifyCall(string time)
        {
            using (var httpClient = new HttpClient())
            {
                var values = new Dictionary<string, string>();
                values.Add("time", time);

                //var response = httpClient.PostAsync(slack_notify_url,
                //new
                //{
                //}, new JsonMediaTypeFormatter()).Result;

                //var contents = response.Content.ReadAsStringAsync().Result;

                var content = new FormUrlEncodedContent(values);
                var response = httpClient.PostAsync(slack_notify_url, content).Result;
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
        #endregion

        private ImageService() { }
    }
}
