using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Electronicute.Allinpay.SDK
{
    public class HttpHelper
    {
        public static async Task<string> HttpGet(string Url, string contentType= "application/json")
        {
            try
            {
                string retString = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = contentType;
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync(); //响应结果
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch
            {
                throw;
            }
        }
        public static async Task<string> HttpPost(string Url, string Content, string contentType= "application/json")
        {
            try
            {
                string result;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                req.Method = "POST";
                req.ContentType = contentType;
                byte[] data = Encoding.UTF8.GetBytes(Content);
                req.ContentLength = data.Length;
                using var reqStream = req.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();
                Stream stream = resp.GetResponseStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                result = reader.ReadToEnd();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
