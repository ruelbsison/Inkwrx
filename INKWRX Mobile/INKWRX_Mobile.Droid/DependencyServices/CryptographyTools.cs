using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Droid.DependencyServices;

[assembly:Xamarin.Forms.Dependency(typeof(CryptographyTools))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    public class CryptographyTools : ICryptography
    {
        public string Decrypt(string encrypted, string firstKey, string secondKey)
        {
            return Destiny.Encryption.CryptoEngine.Decrypt(encrypted, secondKey, firstKey);
        }

        public string Encrypt(string toEncrypt, string firstKey, string secondKey)
        {
            return Destiny.Encryption.CryptoEngine.Encrypt(toEncrypt, secondKey, firstKey);
        }
    }
}