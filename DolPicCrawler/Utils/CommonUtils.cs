using System;
using System.Text;

namespace DolPicCrawler.Utils
{
    class CommonUtils
    {
        public static string getUnicodeToString(string strs)
        {
            //string ret = "";

            Encoding target = Encoding.UTF8;
            byte[] bytes = target.GetBytes(strs);

            var ret = target.GetString(bytes);

            return ret;
        }
    }
}
