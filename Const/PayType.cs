using System;

namespace Electronicute.Allinpay.SDK
{
    /// <summary>
    /// 支付方式(对接文档方便转换)
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 微信支付
        /// </summary>
        VSP501,
        /// <summary>
        /// 微信支付撤销
        /// </summary>
        VSP502,
        /// <summary>
        /// 微信支付退款
        /// </summary>
        VSP503,
        /// <summary>
        /// 支付宝支付
        /// </summary>
        VSP511,
        /// <summary>
        /// 支付宝支付撤销
        /// </summary>
        VSP512,
        /// <summary>
        /// 支付宝支付退款
        /// </summary>
        VSP513,
        /// <summary>
        /// 通联钱包消费
        /// </summary>
        VSP521,
        /// <summary>
        /// 通联钱包消费撤销
        /// </summary>
        VSP522,
        /// <summary>
        /// 通联钱包消费退货
        /// </summary>
        VSP523,
        /// <summary>
        /// 手机QQ支付
        /// </summary>
        VSP505,
        /// <summary>
        /// 手机QQ支付撤销
        /// </summary>
        VSP506,
        /// <summary>
        /// 手机QQ支付退款
        /// </summary>
        VSP507,
        /// <summary>
        /// 银联扫码支付
        /// </summary>
        VSP551,
        /// <summary>
        /// 银联扫码撤销
        /// </summary>
        VSP552,
        /// <summary>
        /// 银联扫码退货
        /// </summary>
        VSP553
    }
}
