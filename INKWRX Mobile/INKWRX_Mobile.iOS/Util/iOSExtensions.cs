using Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace INKWRX_Mobile.iOS.Util
{
    public static class iOSExtensions
    {
        public static DateTime AsDateTime(this NSDate source)
        {
            var refDate = new DateTime(2001, 1, 1);
            var currentDate = refDate.AddSeconds(source.SecondsSinceReferenceDate);
            var local = currentDate.ToLocalTime();

            return local;
        }

        public static byte[] ToByteArray(this NSData data)
        {
            var dataBytes = new byte[data.Length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
            return dataBytes;
        }
    }
}
