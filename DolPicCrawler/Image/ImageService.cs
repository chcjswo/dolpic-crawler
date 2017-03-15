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
        /// <param name="a_dImage">저장할 이미지 정보 Dictionary</param>
        /// <param name="a_dCaption">저장할 내용 정보 Dictionary</param>
        /// <param name="a_nTagUrlType">사이트 타입 1:트위터 2:인스타그램 3:페이스북</param>
        public async void ImageSend(
                Dictionary<string, HashTagQueryData> a_dImage,
                Dictionary<string, HashTagQueryData> a_dCaption,
                int a_nTagUrlType
            )
        {
            using (var client = new HttpClient())
            {
                foreach (KeyValuePair<string, HashTagQueryData> kvp in a_dImage)
                //for (int i = 0; i < a_dImage.Count; i++)
                {
                    //KeyValuePair<string, List<string>> kvp = a_dImage[i];

                    //foreach (var item in kvp.Value.imageSrc)
                    for (var i=0; i<kvp.Value.imageSrc.Count; i++)
                    {
                        // Bsee64 인코딩
                        var sBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(kvp.Value.imageSrc[i]));
                        var values = new Dictionary<string, string>();
                        values.Add("hashTagId", kvp.Key);
                        values.Add("urlType", a_nTagUrlType.ToString());
                        values.Add("imageUrl", sBase64);
                        var content = new FormUrlEncodedContent(values);
                        var result = await client.PostAsync(image_insert_url, content);

                        Console.WriteLine("url == " + string.Format(image_insert_url, kvp.Key, sBase64, a_nTagUrlType, 1));
                        Console.WriteLine("TagNo == " + kvp.Key);
                        Console.WriteLine("ImageSrc == " + sBase64);
                        Console.WriteLine("caption == " + CommonUtils.getUnicodeToString(kvp.Value.captionString[i]));
                        Console.WriteLine("result == " + result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }
        #endregion

        private ImageService() { }
    }
}
