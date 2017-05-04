using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        public static string getUnicodeToString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            Encoding target = Encoding.UTF8;
            byte[] bytes = target.GetBytes(str);

            return target.GetString(bytes);
        }

        public static string getStringToBase64(string str)
        {

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                }
            );
        }

    }
}
