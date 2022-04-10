using Foundation;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Reachability.Net;

[assembly:Xamarin.Forms.Dependency(typeof(DeviceDetailsService))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class DeviceDetailsService : IDeviceDetails
    {
        public string GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }

        public string GetDeviceMake()
        {
            return "Apple";
        }

        public string GetDeviceModel()
        {
            return UIDevice.CurrentDevice.Model;
        }

        public string GetPlatformVersion()
        {
            var info = new NSProcessInfo();
            var os = info.OperatingSystemVersion;
            return string.Format("{0}.{1}.{2}", os.Major, os.Minor, os.PatchVersion);
        }

        public string GetVersionNumber()
        {
            var ret = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            if (CoreAppTools.CurrentServer == CoreAppTools.Server.Prelive)
            {
                ret = string.Format("{0}.{1}", ret, NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString());
            }
            return ret;
        }

        public bool HasInternetConnection(string url)
        {
            var reachability = new Reachability.Net.XamarinIOS.Reachability();
            return reachability.IsHostReachable(url);
        }
    }
}
