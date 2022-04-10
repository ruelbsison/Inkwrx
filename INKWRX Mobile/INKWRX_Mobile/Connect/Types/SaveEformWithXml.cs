using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class SaveEformWithXml : SecureObject
    {
        public SaveEformWithXml(string username, string password, string xmlData, string penData, int appKey, string transXml) :
            base("saveeformwithxml", username, password)
        {
            this.XmlData = xmlData;
            this.PenData = penData;
            this.AppKey = appKey;
            this.TransactonXml = transXml;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("xmldata", new XCData(this.XmlData)),
                new XElement("pendata", new XCData(this.PenData)),
                new XElement("applicationkey", this.AppKey),
                new XElement("transactionxmldata", new XCData(this.TransactonXml))
                );
        }

        public string XmlData { get; set; }
        public string PenData { get; set; }
        public int AppKey { get; set; }
        public string TransactonXml { get; set; }
    }
}
