using System;
using System.Drawing;
using ZXing;
using ZXing.QrCode;

namespace Electronicute.Allinpay.SDK.App
{
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
                return $"data:image/png;base64,{Service.Pic.ToBase64(map)}";
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
}
