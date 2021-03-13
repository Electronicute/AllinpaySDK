using System;
using System.Collections.Generic;
using System.Text;

namespace Electronicute.Allinpay.SDK
{
    /// <summary>
    /// 枚举的交易返回码
    /// </summary>
    [Flags]
    public enum ResultCode
    {
        /// <summary>
        /// 交易成功
        /// </summary>
        OK = 0,
        /// <summary>
        /// 交易不存在
        /// </summary>
        NotFound = 1001,
        /// <summary>
        /// 交易处理中
        /// </summary>
        Waiting = 2008,
        /// <summary>
        /// 交易处理中
        /// </summary>
        ResultGathering = 2000,
        /// <summary>
        /// 流水号重复
        /// </summary>
        BizseqRepeated = 3888,
        /// <summary>
        /// 交易控制失败,具体原因查看errmsg
        /// </summary>
        ControlFail = 3889,
        /// <summary>
        /// 渠道商户错误
        /// </summary>
        CusErr = 3099,
        /// <summary>
        /// 交易金额小于应收手续费
        /// </summary>
        AmountErr = 3014,
        /// <summary>
        /// 校验实名信息失败
        /// </summary>
        AuthFail = 3031,
        /// <summary>
        /// 交易未支付
        /// </summary>
        NotPayed = 3088,
        /// <summary>
        /// 撤销异常
        /// </summary>
        RefundErr = 3089,
        /// <summary>
        /// 交易被撤销
        /// </summary>
        Revoke = 3050,
        /// <summary>
        /// 其他错误
        /// </summary>
        OtherErr1 = 3045,
        /// <summary>
        /// 其他错误
        /// </summary>
        OtherErr2 = 3999,
        /// <summary>
        /// 其他错误
        /// </summary>
        OtherErr
    }
}
