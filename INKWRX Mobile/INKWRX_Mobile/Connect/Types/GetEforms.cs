using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class GetEforms : SecureObject
    {
        public GetEforms(string username, string password) : base("geteforms", username, password)
        {

        }

        internal override void AddFields(XElement element)
        {
            element.Add(new XElement("applicationtype", "Mobile"));
        }
    }
}
