using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class SendFilePacketWithTablet : SecureObject
    {
        public SendFilePacketWithTablet(string username, string password, string fileData, int packetIndex = -1) : base("sendfilepacketwithtablet", username, password)
        {
            this.FileData = fileData;
            this.PacketIndex = packetIndex;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("filedata", this.FileData),
                new XElement("packetindex", this.PacketIndex)
                );
        }

        public string FileData { get; set; }
        public int PacketIndex { get; set; }
    }
}
