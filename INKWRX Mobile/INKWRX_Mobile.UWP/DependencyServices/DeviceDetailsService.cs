using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;

[assembly:Xamarin.Forms.Dependency(typeof(DeviceDetailsService))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class DeviceDetailsService : IDeviceDetails
    {
        public string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dr = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
            byte[] bytes = new byte[hardwareId.Length];
            dr.ReadBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public string GetDeviceMake()
        {
            return "Microsoft";
        }

        public string GetDeviceModel()
        {
            return "Windows";
        }

        public string GetPlatformVersion()
        {
            return "Windows 10";
        }

        public string GetVersionNumber()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            var ret = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            if (CoreAppTools.CurrentServer == CoreAppTools.Server.Prelive)
            {
                ret = string.Format("{0}.{1}", ret, version.Revision);
            }
            return ret;

        }

        public bool HasInternetConnection(string url)
        {
            return false;
        }
    }
}
