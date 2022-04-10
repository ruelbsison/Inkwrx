using FormTools.FormDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile
{
    public static class CoreAppTools
    {
        #region Extension Methods

        public static Color ToColor(this ElementDescriptor.RGBColour colour)
        {
            return Color.FromRgba(colour.Red / 255d, colour.Green / 255d, colour.Blue / 255d, colour.Alpha / 255d);
        }

        public static bool ContainsCharacter(this string source, string search)
        {
            return source.IndexOfAny(search.ToCharArray()) != -1;
        }

        public static bool IsOnlyCharacters(this string source, string search)
        {
            foreach (var c in source.ToCharArray().Select(x=>x.ToString()).Distinct().ToList())
            {
                if (!search.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        public static Color SteelBlue = Color.FromRgb(77d / 255d, 134d / 255d, 142d / 255d);
        public static Color CalculationGreen = Color.FromRgb(109d / 255d, 205d / 255d, 177d / 255d);
        public static Color PrepopBlue = Color.FromRgb(40d / 255d, 98d / 255d, 142d / 255d);
        public static Color MandatoryRed = Color.FromRgb(248d / 255d, 158d / 255d, 163d / 255d);
        public static Color SelectorGrey = Color.FromRgb(224d / 255d, 224d / 255d, 224d / 255d);
        public static Color LightSilver = Color.FromRgb(224d / 255d, 224d / 255d, 224d / 255d);

        public static ImageSource GetImageSource(string fileName)
        {
            return Device.OnPlatform (
                iOS: ImageSource.FromFile(fileName),
                Android: ImageSource.FromFile(fileName.Substring(fileName.LastIndexOf('/') + 1)),
                WinPhone: ImageSource.FromFile(fileName)
                );
        }

        public static Server CurrentServer = Server.Prelive;

        public enum Server
        {
            Live,
            Prelive,
            Dev,
            N3,
            Inx
        }

        public enum Service
        {
            Form,
            ServiceCenter
        }

        public static string GetVersionNumber()
        {
            var assembly = typeof(CoreAppTools).GetTypeInfo().Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);
            var version = assemblyName.Version;
            var ret = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            if (CoreAppTools.CurrentServer == Server.Prelive)
            {
                ret = string.Format("{0}.{1}", ret, version.Revision);
            }
            return ret;
        }

        public static Dictionary<Server, Dictionary<Service, string>> Servers = new Dictionary<Server, Dictionary<Service, string>>
        {
            {
                Server.Live, new Dictionary<Service, string>
                {
                    { Service.Form, "https://cloud.inkwrx.com/formmanagersec/service/DestFormServiceSec.svc" },
                    { Service.ServiceCenter, "https://cloud.inkwrx.com/servicecentersec/service/SvcCenterSecService.svc" }
                }
            },
            {
                Server.Prelive, new Dictionary<Service, string>
                {
                    { Service.Form, "https://prelive.inkwrx.com/formmanagersec/service/DestFormServiceSec.svc" },
                    { Service.ServiceCenter, "https://prelive.inkwrx.com/servicecentersec/service/SvcCenterSecService.svc" }
                }
            },
            {
                Server.Dev, new Dictionary<Service, string>
                {
                    { Service.Form, "http://inkworksdev.destinywireless.com/formmanagersec/service/DestFormServiceSec.svc" },
                    { Service.ServiceCenter, "http://inkworksdev.destinywireless.com/servicecentersec/service/SvcCenterSecService.svc" }
                }
            },
            {
                Server.N3, new Dictionary<Service, string>
                {
                    { Service.Form, "https://mobileinkworksn3.destinywireless.com/formmanagersec/service/DestFormServiceSec.svc" },
                    { Service.ServiceCenter, "https://mobileinkworksn3.destinywireless.com/servicecentersec/service/SvcCenterSecService.svc" }
                }
            },
            {
                Server.Inx, new Dictionary<Service, string>
                { // TODO: Update this if we need Inx...
                    { Service.Form, "https://cloud.inkwrx.com/formmanagersec/service/DestFormServiceSec.svc" },
                    { Service.ServiceCenter, "https://cloud.inkwrx.com/servicecentersec/service/SvcCenterSecService.svc" }
                }
            }

        };
    }
}
