using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Prepop
{
    public class PrepopFormWrapper
    {
        public PrepopFormWrapper(XElement xml)
        {
            var idKey = xml.Attribute("id_key");
            if (idKey != null)
            {
                this.PrepopId = int.Parse(idKey.Value);
            }
            var appKey = xml.Attribute("app_key");
            if (appKey != null)
            {
                this.AppKey = int.Parse(appKey.Value);
            }
            var prepopName = xml.Element("identifier");
            if (prepopName != null)
            {
                this.PrepopName = prepopName.Value;
            }
            this.Fields = new List<PrepopFieldWrapper>();
            var data = xml.Element("data");
            if (data != null)
            {
                var record = data.Element("record");
                if (record != null)
                {
                    foreach (var elem in record.Elements())
                    {
                        this.Fields.Add(new PrepopFieldWrapper(elem));
                    }
                }
            }
        }


        public List<PrepopFieldWrapper> Fields { get; set; }
        public int PrepopId { get; set; }
        public string PrepopName { get; set; }
        public int AppKey { get; set; }
    }
}
