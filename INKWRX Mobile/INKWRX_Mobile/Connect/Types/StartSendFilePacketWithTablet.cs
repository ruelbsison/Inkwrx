using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class StartSendFilePacketWithTablet : SecureObject
    {
        public StartSendFilePacketWithTablet(string username, string password, string filename, int maxPackets = 0, int blockSize = 0, int fileSize = 0) :
            base("startsendfilepacketwithtablet", username, password)
        {
            this.FileName = filename;
            this.MaxPackets = maxPackets;
            this.BlockSize = blockSize;
            this.FileSize = fileSize;
        }

        internal override void AddFields(XElement element)
        {
            element.Add(
                new XElement("filename", this.FileName),
                new XElement("maxpackets", this.MaxPackets),
                new XElement("blocksize", this.BlockSize),
                new XElement("filesize", this.FileSize)
                );
        }

        public string FileName { get; set; }
        public int MaxPackets { get; set; }
        public int BlockSize { get; set; }
        public int FileSize { get; set; }
    }
}
