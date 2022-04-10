using INKWRX_Mobile.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace INKWRX_Mobile.Connect.Types
{
    public class SecureObject
    {
        public SecureObject(string functionName, string username, string password)
        {
            this.FunctionName = functionName;
            this.Username = username;
            this.Password = password;
        }

        private XElement GetXmlDeviceTag()
        {
            return new XElement("device",
                new XElement("os", Device.OnPlatform("iOS", "Android", "Windows")),
                new XElement("osversion", DependencyService.Get<IDeviceDetails>().GetPlatformVersion()),
                new XElement("make", DependencyService.Get<IDeviceDetails>().GetDeviceMake()),
                new XElement("model", DependencyService.Get<IDeviceDetails>().GetDeviceModel()),
                new XElement("tabletid", DependencyService.Get<IDeviceDetails>().GetDeviceId())
                );
        }

        protected internal XElement ToXml()
        {
            var elem = new XElement("data",
                new XAttribute("messageversion", "2"),
                this.GetXmlDeviceTag(),
                new XElement("function", this.FunctionName),
                new XElement("username", this.Username),
                new XElement("password", this.Password)
                );
            this.AddFields(elem);
            return elem;
        }

        internal virtual void AddFields(XElement element)
        {

        }

        public string FunctionName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
