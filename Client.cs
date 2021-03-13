using Electronicute.Allinpay.SDK.Encry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace Electronicute.Allinpay.SDK
{
    /// <summary>
    /// 静态类接口
    /// </summary>
    public class AllinpayClient
    {
        public static string cusid;
        public static string appid;
        public static string bca;
        public static string md5;
        public static string RSAPrivateKey;
        public static string RSAPublicKey;
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
    /// <summary>
    /// 当面付的快捷应用接口
    /// </summary>
    public class ParameteredPay : IDisposable
    {
        private string urlPos;
        /// <summary>
        /// 构造一个当面付的快捷应用
        /// </summary>
        /// <param name="ac">一个通联支付的客户端实例</param>
        /// <param name="amt">金额(单位:分)</param>
        /// <param name="oid">订单号</param>
        /// <param name="trxreserve">备注</param>
        /// <param name="_isINSDMF">是否真实验证人付款</param>
        public ParameteredPay(long amt, string oid, string trxreserve = null, bool _isINSDMF = false) => this.urlPos = AllinpayClient.ParameteredPay(amt, oid, trxreserve, _isINSDMF);
        /// <summary>
        /// 生成二维码的Base64
        /// </summary>
        /// <param name="width">宽度(默认200)</param>
        /// <param name="height">高度(默认200)</param>
        /// <returns></returns>
        public string GenerateBarCode(int width = 200, int height = 200)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        DisableECI = true,
                        CharacterSet = "UTF-8",
                        Width = width,
                        Height = height,
                        Margin = 1
                    }
                };
                using Bitmap map = writer.Write(urlPos);
                return $"data:image/png;base64,{App.Pic.ToBase64(map)}";
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 析构方案
        /// </summary>
        public void Dispose() => GC.Collect();
    }
    /// <summary>
    /// 检查支付
    /// </summary>
    public class CheckPay : IDisposable
    {
        public JObject jo { get; set; }
        public bool _isOffical { get; set; }
        /// <summary>
        /// 获取一个订单
        /// </summary>
        /// <param name="trxid">流水号</param>
        /// <param name="trxdate">日期</param>
        /// <param name="orderid">商户订单号</param>
        /// <param name="resendNotify">是否重新发送</param>
        /// <returns></returns>
        public async Task<CheckPay> Get(string trxid = null, int trxdate = 0101, string orderid = null, bool resendNotify = false)
        {
            jo = await AllinpayClient.CheckTransaction(trxdate, trxid, orderid, resendNotify);
            _isOffical = Verify();
            return this;
        }
        /// <summary>
        /// 验签
        /// </summary>
        /// <returns>是否为真实值</returns>
        private bool Verify()
        {
            StringBuilder sb = new StringBuilder();
            string sign="";
            bool b = false;
            foreach(var (k,v) in jo)
            {
                if (k.Equals("sign"))
                {
                    sign = v.ToString();
                    continue;
                }
                if (b)
                {
                    sb.Append("&");
                }
                sb.Append($"{k}={v}");
                b = true;
            }
            return RSA.AllinpayVerify(sb.ToString(),sign);
        }
        /// <summary>
        /// 适应using写法的析构
        /// </summary>
        public void Dispose() => GC.Collect();
    }
}
