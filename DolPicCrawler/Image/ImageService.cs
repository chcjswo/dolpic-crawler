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

        #region 이미지 저장하기
        /// <summary>
        /// 이미지 저장하기
        /// </summary>
        /// <param name="a_dImage">저장할 이미지 정보 Dictionary</param>
        /// <param name="a_nTagUrlType">사이트 타입 1:트위터 2:인스타그램 3:페이스북</param>
        public async void ImageSend(Dictionary<string, List<string>> a_dImage, int a_nTagUrlType)
        {
            using (var client = new HttpClient())
            {
                foreach (KeyValuePair<string, List<string>> kvp in a_dImage)
                {
                    foreach (var item in kvp.Value)
                    {
                        // Bsee64 인코딩
                        var sBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(item));
                        var values = new Dictionary<string, string>();
                        values.Add("hashTagId", kvp.Key);
                        values.Add("urlType", a_nTagUrlType.ToString());
                        values.Add("imageUrl", sBase64);
                        var content = new FormUrlEncodedContent(values);
                        var result = await client.PostAsync(image_insert_url, content);

                        Console.WriteLine("url == " + string.Format(image_insert_url, kvp.Key, sBase64, a_nTagUrlType, 1));
                        Console.WriteLine("TagNo == " + kvp.Key);
                        Console.WriteLine("ImageSrc == " + sBase64);
                        Console.WriteLine("result == " + result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }
        #endregion

        private ImageService() { }
    }
}
