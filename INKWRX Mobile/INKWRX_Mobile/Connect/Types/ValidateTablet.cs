using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Connect.Types
{
    public class ValidateTablet : SecureObject
    {
        public ValidateTablet(string username, string password) : base("validatetablet", username, password)
        {

        }
    }
}
