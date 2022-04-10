using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Views;
using System.Reflection;

namespace INKWRX_Mobile.Util
{
    internal static class Crypto
    {
        private static string SecretKey;

        private static string ReadFile()
        {

            var assembly = typeof(FormViewPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("INKWRX_Mobile.SecretKey.txt");
            var text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        internal static string GetFormattedDate(DateTime date)
        {
            return date.ToString("ddMMyyyy HHmmss");
        }

        internal static string Encrypt(string toEncrypt, string firstKey)
        {
            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = ReadFile().Replace("\n", "").Replace("\r", "").Trim();
            }
            return DependencyService.Get<ICryptography>().Encrypt(toEncrypt, firstKey, SecretKey);
        }

        internal static string Decrypt(string toDecrypt, string firstKey)
        {
            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = ReadFile().Replace("\n", "").Replace("\r", "").Trim();
            }
            return DependencyService.Get<ICryptography>().Decrypt(toDecrypt, firstKey, SecretKey);
        }


    }
}
