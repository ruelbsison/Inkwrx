using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly:Xamarin.Forms.Dependency(typeof(CryptographyTools))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class CryptographyTools : ICryptography
    {
        private static string getCryptoKey(string date, string secretKey)
        {
            if (string.IsNullOrEmpty(date))
            {
                return secretKey;
            }
            var split = date.Split(' ');
            if (split.Length != 2)
            {
                return secretKey;
            }

            return string.Format("{0}{1}{2}", split[1], secretKey, split[0]);
        }

        /// <summary>
        /// Hashes a key and returns a byte array of a given length
        /// </summary>
        /// <param name="key">Key to hash</param>
        /// <param name="bytelength">Length of byte array to return</param>
        /// <returns>Byte array of given length for hashed value</returns>
        private static byte[] GetHashedKey(string key, int bytelength)
        {
            var msSha256 = SHA256.Create();
            var tempArray = msSha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            //msSha256.Clear();

            var keyArray = new byte[bytelength];
            Buffer.BlockCopy(tempArray, 0, keyArray, 0, 24);
            return keyArray;
        }

        /// <summary>
        /// Encrypts a string using dual-encryption method using 2 keys.
        /// </summary>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="firstkey">First key.</param>
        /// <param name="secondkey">Second key.</param>
        /// <returns>Encrypted string</returns>
        private static string encrypt(string toEncrypt, string firstkey, string secondkey)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            var tdes = TripleDES.Create();
            tdes.Key = GetHashedKey(getCryptoKey(firstkey, secondkey), 24);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">Encrypted string</param>
        /// <param name="firstkey">First part of the key</param>
        /// <param name="secondkey">Second part of the key</param>
        /// <returns>Decrypted string</returns>
        private static string decrypt(string cipherString, string firstkey, string secondkey)
        {
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            var tdes = TripleDES.Create();
            tdes.Key = GetHashedKey(getCryptoKey(firstkey, secondkey), 24);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            
            return Encoding.UTF8.GetString(resultArray);
        }

        public string Decrypt(string encrypted, string firstKey, string secondKey)
        {
            return decrypt(encrypted, firstKey, secondKey);
        }

        public string Encrypt(string toEncrypt, string firstKey, string secondKey)
        {
            return encrypt(toEncrypt, firstKey, secondKey);

        }
    }
}
