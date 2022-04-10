using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class SendFileWithTablet : SecureObject
    {
        public SendFileWithTablet(string username, string password, string filename, string fileData) : base("sendfilewithtablet", username, password)
        {
            this.FileName = filename;
            this.FileData = fileData;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("filename", this.FileName),
                new XElement("filedata", this.FileData)
                );
        }

        public string FileName { get; set; }
        public string FileData { get; set; }
    }
}
