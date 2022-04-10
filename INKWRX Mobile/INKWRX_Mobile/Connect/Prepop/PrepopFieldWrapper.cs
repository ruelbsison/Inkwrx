using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace INKWRX_Mobile.Connect.Prepop
{
    public class PrepopFieldWrapper
    {
        public PrepopFieldWrapper(XElement xml)
        {
            var fieldName = xml.Attribute("name");
            if (fieldName != null)
            {
                this.FieldName = fieldName.Value;
            }
            this.FieldValue = string.IsNullOrEmpty(xml.Value) ? "" : xml.Value;
        }

        public string FieldName { get; set; }
        
        public string FieldValue { get; set; }
    }
}
