using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Types
{
    public class ResponseItem
    {
        public ResponseItem(XElement xml)
        {
            this.ErrorCode = xml.Element("errorcode") == null ? -1 : int.Parse(xml.Element("errorcode").Value);
            this.Message = xml.Element("message") == null ? "" : xml.Element("message").Value;
            this.ConfigV = xml.Element("configv") == null ? "" : xml.Element("configv").Value;
            this.AppParamVersion = xml.Element("appparamversion") == null ? "" : xml.Element("appparamversion").Value;
            this.PrintParamVersion = xml.Element("printparamversion") == null ? "" : xml.Element("printparamversion").Value;
            this.Restart = xml.Element("restart") == null ? false : xml.Element("restart").Value == "true";
            this.NextPacketId = xml.Element("nextpacketid") == null ? -1 : int.Parse(xml.Element("nextpacketid").Value);
            this.FileData = xml.Element("filedata") == null 
                ? xml.Element("prepopdata") == null 
                    ? "" 
                    : xml.Element("prepopdata").Value 
                : xml.Element("filedata").Value;
            this.PrepopVersion = xml.Element("versionnumber") == null ? -1 : int.Parse(xml.Element("versionnumber").Value);
            this.ByteData = xml.Element("bytedata") == null ? "" : xml.Element("bytedata").Value;
        }

        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string ConfigV { get; set; }
        public string AppParamVersion { get; set; }
        public string PrintParamVersion { get; set; }
        public bool Restart { get; set; }
        public int NextPacketId { get; set; }
        public string FileData { get; set; }
        public int PrepopVersion { get; set; }
        public string ByteData { get; set; }
    }
}
