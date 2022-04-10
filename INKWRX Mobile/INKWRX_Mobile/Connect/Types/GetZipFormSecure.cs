using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class GetZipFormSecure : SecureObject
    {
        public GetZipFormSecure(string username, string password, int appId) : base ("getzipformsecure", username, password)
        {
            this.ApplicationId = appId;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("applicationkey", this.ApplicationId),
                new XElement("fulllexicon", "true")
                );
        }

        public int ApplicationId { get; set; }
    }
}
