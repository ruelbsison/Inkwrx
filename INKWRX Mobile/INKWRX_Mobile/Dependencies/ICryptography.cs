using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Dependencies
{
    public interface ICryptography
    {
        string Encrypt(string toEncrypt, string firstKey, string secondKey);
        string Decrypt(string encrypted, string firstKey, string secondKey);
    }
}
