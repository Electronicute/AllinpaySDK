using System;
using System.Collections.Generic;
using System.Text;

namespace Electronicute.Allinpay.SDK.Model
{
    public class Retpg
    {
        public string appid;
        public string c;
        public string oid;
        public string amt;
        public string randomstr;
        public string trxreserve;
        public string sign;

        public Retpg(
            string appid, string c, string oid, 
            string amt, string randomstr, 
            string trxreserve, string sign)
        {
            this.appid = appid;
            this.c = c;
            this.oid = oid;
            this.amt = amt;
            this.randomstr = randomstr;
            this.trxreserve = trxreserve;
            this.sign = sign;
        }
    }
}
