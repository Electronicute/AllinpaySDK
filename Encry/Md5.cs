using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Electronicute.Allinpay.SDK.Encry
{
    /// <summary>
    /// Md5加密(
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>加密后字符串</returns>
        public static string GenerateMD5(string txt)
        {
            using MD5 mi = MD5.Create();
            byte[] buffer = Encoding.Default.GetBytes(txt);
            byte[] newBuffer = mi.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// MD5流加密
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static string GenerateMD5(Stream inputStream)
        {
            using MD5 mi = MD5.Create();
            byte[] newBuffer = mi.ComputeHash(inputStream);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
