using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Prepop
{
    public class PrepopWrapper
    {
        public PrepopWrapper(XElement xml, int version)
        {
            this.Version = version;
            this.Additions = new List<PrepopFormWrapper>();
            this.Removals = new List<PrepopFormWrapper>();
            var additions = xml.Element("additions");
            if (additions != null)
            {
                foreach (var elem in additions.Elements().ToList())
                {
                    this.Additions.Add(new PrepopFormWrapper(elem));
                }
            }
            var removals = xml.Element("removals");
            if (removals != null)
            {
                foreach (var elem in removals.Elements().ToList())
                {
                    this.Removals.Add(new PrepopFormWrapper(elem));
                }
            }
        }

        public int Version { get; set; }

        public List<PrepopFormWrapper> Additions { get; set; }
        public List<PrepopFormWrapper> Removals { get; set; }
    }
}
