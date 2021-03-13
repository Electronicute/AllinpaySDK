using Electronicute.Allinpay.SDK.Encry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Electronicute.Allinpay.SDK
{
    /// <summary>
    /// 静态类接口
    /// </summary>
    public class AllinpayClient
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public static string cusid;
        /// <summary>
        /// 应用号
        /// </summary>
        public static string appid;
        /// <summary>
        /// 二维码号
        /// </summary>
        public static string bca;
        /// <summary>
        /// 设置的Md5签
        /// </summary>
        public static string md5;
        /// <summary>
        /// RSA的私钥
        /// </summary>
        public static string RSAPrivateKey;
        /// <summary>
        /// RSA的公钥
        /// </summary>
        public static string RSAPublicKey;
        /// <summary>
        /// 支付的跳转URL
        /// </summary>
        public static string returl = null;
        /// <summary>
        /// 异步查询一个交易
        /// </summary>
        /// <param name="trxdate">日期</param>
        /// <param name="trxid">流水号</param>
        /// <param name="orderid">商户单号</param>
        /// <param name="_resendNotify">是否重发通知</param>
        /// <returns>一个交易Json</returns>
        public static async Task<JObject> CheckTransaction(int trxdate, string trxid = null, string orderid = null, bool _resendNotify = false) => JObject.Parse(await HttpHelper.HttpPost(RSA.SignUrl("https://vsp.allinpay.com/apiweb/tranx/queryorder?",
            $"appid={appid}&cusid={cusid}{(orderid != null ? $"&orderid={orderid}" : "")}&randomstr={new Random().Next(5000).ToString().PadLeft(5, '0')}" +
            $"&resendnotify={(_resendNotify ? "1" : "0")}&signtype=RSA&termno={bca}&trxdate={trxdate.ToString().PadLeft(4, '0')}{(trxid != null ? $"& trxid={trxid}" : "")}"
            , RSAPrivateKey), ""));
        /// <summary>
        /// 带参数的当面付调用
        /// </summary>
        /// <param name="amt">金额(单位:分)</param>
        /// <param name="oid">商户订单号</param>
        /// <param name="trxreserve">识别信息(详见官方开发文档)</param>
        /// <param name="_isINSDMF">是否实名支付(根据类型选)</param>
        /// <returns>生成的url,可以发给客户</returns>
        public static string ParameteredPay(long amt, string oid, string trxreserve = null, bool _isINSDMF = false) =>
            $"{(_isINSDMF ? "https://syb.allinpay.com/apiweb/insdmf/cuspay" : "https://syb.allinpay.com/sappweb/usertrans/cuspay?")}amt={amt}&appid={appid}&c={bca}&key={md5}&oid={oid}" +
            $"{(returl != null ? ($"&returl={System.Web.HttpUtility.UrlEncode(returl, Encoding.UTF8)}") : "")}&signtype=MD5{(trxreserve != null ? $"&trxreserve={System.Web.HttpUtility.UrlEncode(trxreserve, Encoding.UTF8)}" : "")}" +
            $"&sign={Md5.GenerateMD5($"amt={amt}&appid={appid}&c={bca}&key={md5}&oid={oid}{(returl != null ? ($"&returl={returl}") : "")}&signtype=MD5{(trxreserve != null ? $"&trxreserve={trxreserve}" : "")}").ToUpper()}";
    }
}
