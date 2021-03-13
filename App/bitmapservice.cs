using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Electronicute.Allinpay.SDK.App
{
    public class Pic
    {
        public static string ToBase64(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch
            {
                throw;
            }
        }
    }
}
