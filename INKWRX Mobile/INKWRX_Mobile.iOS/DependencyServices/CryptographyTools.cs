using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

[assembly:Xamarin.Forms.Dependency(typeof(CryptographyTools))]
namespace INKWRX_Mobile.iOS.DependencyServices
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


		private static string GetKey(string part1, string part2)
		{
			if (string.IsNullOrEmpty(part2) || part2.Length < 2)
			{
				return part1;
			}

			//should have a space in there if not its unexpected
			if (part2.Contains(" "))
			{
				var parts = part2.Split(' ');
				return parts[1] + part1 + parts[0];
			}

			if (part2.Length > 8)
			{
				return part2.Substring(8) + part1 + part2.Substring(0, 8);
			}

			var half = part2.Length / 2;
			return part2.Substring(half) + part1 + part2.Substring(0, half);
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
			msSha256.Clear();

			var keyArray = new byte[bytelength];
			Buffer.BlockCopy(tempArray, 0, keyArray, 0, 24);
			return keyArray;
		}

		///// <summary>
		///// Encrypts a string using dual-encryption method using 2 keys.
		///// </summary>
		///// <param name="toEncrypt">String to encrypt</param>
		///// <param name="firstkey">First key.</param>
		///// <param name="secondkey">Second key.</param>
		///// <returns>Encrypted string</returns>
		//public string Encrypt(string toEncrypt, string firstkey, string secondkey)
		//{
		//	byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
		//	var tdes = new TripleDESCryptoServiceProvider
		//	{
		//		Key = GetHashedKey(GetKey(firstkey, secondkey), 24),
		//		Mode = CipherMode.ECB,
		//		Padding = PaddingMode.PKCS7
		//	};

		//	ICryptoTransform cTransform = tdes.CreateEncryptor();
		//	byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		//	tdes.Clear();
		//	return Convert.ToBase64String(resultArray, 0, resultArray.Length);

		//}

		///// <summary>
		///// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
		///// </summary>
		///// <param name="cipherString">Encrypted string</param>
		///// <param name="firstkey">First part of the key</param>
		///// <param name="secondkey">Second part of the key</param>
		///// <returns>Decrypted string</returns>
		//public string Decrypt(string cipherString, string firstkey, string secondkey)
		//{
			
		//	byte[] toEncryptArray = Convert.FromBase64String(cipherString);


		//	var tdes = new TripleDESCryptoServiceProvider
		//	{
		//		Key = GetHashedKey(GetKey(firstkey, secondkey), 24),
		//		Mode = CipherMode.ECB,
		//		Padding = PaddingMode.PKCS7
		//	};

		//	ICryptoTransform cTransform = tdes.CreateDecryptor();
		//	byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

		//	tdes.Clear();
		//	return Encoding.UTF8.GetString(resultArray);
		//}
    }
    
}
