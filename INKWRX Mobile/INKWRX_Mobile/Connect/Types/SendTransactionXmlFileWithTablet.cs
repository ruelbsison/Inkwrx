using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class SendTransactionXmlFileWithTablet : SecureObject
    {
        public SendTransactionXmlFileWithTablet(string username, string password, string filename, string transXml) : base("sendtransactionxmlfilewithtablet", username, password)
        {
            this.FileName = filename;
            this.TransactionXml = transXml;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("filename", this.FileName),
                new XElement("transactionxmldata", new XCData(this.TransactionXml))
                );
        }

        public string FileName { get; set; }
        public string TransactionXml { get; set; }
    }
}
