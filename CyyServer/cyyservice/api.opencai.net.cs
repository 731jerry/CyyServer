using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Cache;

namespace OCAPIDemo
{
    public class api
    {
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 15000;//设置10秒超时
            request.Proxy = null;
            request.Method = "GET";
            request.ContentType = "application/x-www-from-urlencoded";
            request.ServicePoint.Expect100Continue = false;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));
            StringBuilder sb = new StringBuilder();
            sb.Append(reader.ReadToEnd());
            request.Abort();
            reader.Close();
            reader.Dispose();
            response.Close();
            return sb.ToString();
        }

        public static string LinkDataGrab(string link)
        {
            string cpData;

            WebRequest request = WebRequest.Create(link);
            request.Timeout = 45000;//设置超时
            WebResponse response = request.GetResponse();
            StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
            cpData = stream.ReadToEnd();//
            //request.Abort();
            stream.Close();
            stream.Dispose();
            response.Close();

            return cpData;
        }
    }
}
