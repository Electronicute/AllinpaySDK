using Electronicute.Allinpay.SDK.Encry;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Electronicute.Allinpay.SDK.App
{
    /// <summary>
    /// 检查支付接口
    /// </summary>
    public class CheckPay : IDisposable
    {
        /// <summary>
        /// 元返回量
        /// </summary>
        public JObject jo { get; set; }
        /// <summary>
        /// 验签标准(True则为验签成功)
        /// </summary>
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
