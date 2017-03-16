using System.Text;

namespace DolPicCrawler.Utils
{
    /// <summary>
    /// 공통 유틸 클래스
    /// </summary>
    class CommonUtils
    {
        /// <summary>
        /// unicode를 string으로 변환해서 반환
        /// </summary>
        /// <param name="strs">변환 할 unicode</param>
        /// <returns></returns>
        public static string getUnicodeToString(string strs)
        {
            if (string.IsNullOrEmpty(strs)) return "";

            Encoding target = Encoding.UTF8;
            byte[] bytes = target.GetBytes(strs);

            return target.GetString(bytes);
        }
    }
}
