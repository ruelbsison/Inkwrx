using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class GetEformPrepopDataForUser : SecureObject
    {
        public GetEformPrepopDataForUser(string username, string password, int currentVersion) : base("geteformprepopdataforuser", username, password)
        {
            this.CurrentVersion = currentVersion;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(new XElement("currentversion", this.CurrentVersion));
            element.Add(new XElement("applicationtype", "Mobile"));
        }

        public int CurrentVersion { get; set; }
    }
}
