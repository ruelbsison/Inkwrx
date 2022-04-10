using System;

using Android.App;
using Android.OS;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Droid.DependencyServices;
using Android.Provider;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceDetailsService))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    public class DeviceDetailsService : IDeviceDetails
    {
        public string GetDeviceId()
        {
            return !Build.Serial.Equals(Build.Unknown) ? Build.Serial
                : Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);
        }

        public string GetPlatformVersion()
        {
            return "Android " + Build.VERSION.Release;
        }

        public string GetVersionNumber()
        {
            Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (CoreAppTools.Server.Prelive == CoreAppTools.CurrentServer)
            {
                return string.Format("{0}.{1}.{2}.{3}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
            }
            else
            {
                return string.Format("{0}.{1}.{2}", appVersion.Major, appVersion.Minor, appVersion.Build);
            }
        }

        public string GetDeviceMake()
        {
            return Build.Manufacturer;
        }

        public string GetDeviceModel()
        {
            return Build.Model;
        }

        public bool HasInternetConnection(string url)
        {
            var reach = new Reachability.Net.XamarinAndroid.Reachability();
            return reach.IsHostReachable(url);
        }
    }
}