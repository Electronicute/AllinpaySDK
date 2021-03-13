using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Electronicute.Allinpay.SDK.Encry
{
    /// <summary>
    /// RSA加密
    /// </summary>
    public class RSA
    {
        /// <summary>
        /// 通联签名URL
        /// </summary>
        /// <param name="Header">头部</param>
        /// <param name="Body">查询串</param>
        /// <param name="RSAprivate_PCKS8">RSA私钥</param>
        /// <returns></returns>
        public static string SignUrl(string Header, string Body,string RSAprivate_PCKS8) => $"{Header}{Body}&sign={System.Web.HttpUtility.UrlEncode(RSA.Sign(Body, RSAprivate_PCKS8), System.Text.Encoding.UTF8)}";
        /// <summary>
        /// 文档生产公钥
        /// </summary>
        public const string AllinpayPublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCm9OV6zH5DYH/ZnAVYHscEELdCNfNTHGuBv1nYYEY9FrOzE0/4kLl9f7Y9dkWHlc2ocDwbrFSm0Vqz0q2rJPxXUYBCQl5yW3jzuKSXif7q1yOwkFVtJXvuhf5WRy+1X5FOFoMvS7538No0RpnLzmNi3ktmiqmhpcY/1pmt20FHQQIDAQAB";
        /// <summary>
        /// 通联生产模式验证公钥函数
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="signedString">加密字段</param>
        /// <returns></returns>
        public static bool AllinpayVerify(string content, string signedString) => Verify(content, signedString, AllinpayPublicKey);
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string Sign(string content, string privateKey)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
                byte[] signData = rsa.SignData(data, new SHA1CryptoServiceProvider());
                return Convert.ToBase64String(signData);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// RSA验证
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="signedString">已签名字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool Verify(string content, string signedString, string publicKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] soureData = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            return rsaPub.VerifyData(data, new SHA1CryptoServiceProvider(), soureData);
        }

        #region RSA协议部分对接Java
        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            RSACryptoServiceProvider rsacsp = null;

            int lenStream = (int)mem.Length;
            BinaryReader binReader = new BinaryReader(mem);
            byte bt = 0;
            ushort twoBytes = 0;

            try
            {
                twoBytes = binReader.ReadUInt16();
                if (twoBytes == 0x8130)        //data read as little endian order (actual data order for Sequence is 30 81)
                    binReader.ReadByte();    //advance 1 byte
                else if (twoBytes == 0x8230)
                    binReader.ReadInt16();    //advance 2 bytes
                else
                    return null;

                bt = binReader.ReadByte();
                if (bt != 0x02)
                    return null;

                twoBytes = binReader.ReadUInt16();

                if (twoBytes != 0x0001)
                    return null;

                seq = binReader.ReadBytes(15);            //read the Sequence OID
                if (!CompareBytearrays(seq, seqOID))    //make sure Sequence for OID is correct
                    return null;

                bt = binReader.ReadByte();
                if (bt != 0x04)                    //expect an Octet string 
                    return null;

                bt = binReader.ReadByte();        //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binReader.ReadByte();
                else
                    if (bt == 0x82)
                    binReader.ReadUInt16();

                // at this stage, the remaining sequence should be the RSA private key
                byte[] rsaprivkey = binReader.ReadBytes((int)(lenStream - mem.Position));
                rsacsp = DecodeRSAPrivateKey(rsaprivkey);

                return rsacsp;
            }
            catch
            {
                return null;
            }
            finally
            {
                binReader.Close();
            }
        }
        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] modulus, e, d, p, q, dp, dq, iq;

            // Set up stream to decode the asn.1 encoded RSA private key
            MemoryStream mem = new MemoryStream(privkey);

            // wrap Memory Stream with BinaryReader for easy reading
            BinaryReader binReader = new BinaryReader(mem);
            byte bt = 0;
            ushort twoBytes = 0;
            int elems = 0;
            try
            {
                twoBytes = binReader.ReadUInt16();
                if (twoBytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                {
                    binReader.ReadByte(); // advance 1 byte
                }
                else if (twoBytes == 0x8230)
                {
                    binReader.ReadInt16(); // advance 2 byte
                }
                else
                {
                    return null;
                }

                twoBytes = binReader.ReadUInt16();
                if (twoBytes != 0x0102) // version number
                    return null;
                bt = binReader.ReadByte();
                if (bt != 0x00)
                    return null;

                // all private key components are Integer sequences
                elems = GetIntegerSize(binReader);
                modulus = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                e = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                d = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                p = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                q = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                dp = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                dq = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                iq = binReader.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters rsaParams = new RSAParameters();
                rsaParams.Modulus = modulus;
                rsaParams.Exponent = e;
                rsaParams.D = d;
                rsaParams.P = p;
                rsaParams.Q = q;
                rsaParams.DP = dp;
                rsaParams.DQ = dq;
                rsaParams.InverseQ = iq;
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binReader.Close();
            }
        }
        private static int GetIntegerSize(BinaryReader binReader)
        {
            byte bt = 0;
            byte lowByte = 0x00;
            byte highByte = 0x00;
            int count = 0;
            bt = binReader.ReadByte();
            if (bt != 0x02)        //expect integer
                return 0;
            bt = binReader.ReadByte();

            if (bt == 0x81)
            {
                count = binReader.ReadByte();    // data size in next byte
            }
            else
            {
                if (bt == 0x82)
                {
                    highByte = binReader.ReadByte();    // data size in next 2 bytes
                    lowByte = binReader.ReadByte();
                    byte[] modint = { lowByte, highByte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;        // we already have the data size
                }
            }

            while (binReader.ReadByte() == 0x00)
            {    //remove high order zeros in data
                count -= 1;
            }
            binReader.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
        private static RSACryptoServiceProvider DecodePemPrivateKey(string pemstr)
        {
            RSACryptoServiceProvider rsa = null;
            byte[] pkcs8PrivteKey = Convert.FromBase64String(pemstr);
            if (pkcs8PrivteKey != null)
            {
                rsa = DecodePrivateKeyInfo(pkcs8PrivteKey);
            }
            return rsa;
        }
        private static RSAParameters ConvertFromPublicKey(string pempublicKey)
        {
            byte[] keyData = Convert.FromBase64String(pempublicKey);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters
            {
                Modulus = pemModulus,
                Exponent = pemPublicExponent
            };
            return para;
        }
        #endregion
    }
}