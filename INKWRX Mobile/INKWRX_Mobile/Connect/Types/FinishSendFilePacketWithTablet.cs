using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class FinishSendFilePacketWithTablet : SecureObject
    {
        public FinishSendFilePacketWithTablet(string username, string password, string fileName, int maxPackets = -1) : base("finishsendfilepacketwithtablet", username, password)
        {
            this.FileName = fileName;
            this.MaxPackets = maxPackets;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("filename", this.FileName),
                new XElement("maxpackets", this.MaxPackets)
                );
        }

        public string FileName { get; set; }
        public int MaxPackets { get; set; }
    }
}
