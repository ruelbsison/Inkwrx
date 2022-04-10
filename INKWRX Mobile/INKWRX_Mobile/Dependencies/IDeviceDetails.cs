using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Dependencies
{
    public interface IDeviceDetails
    {
        string GetDeviceId();

        string GetVersionNumber();

        string GetPlatformVersion();

        string GetDeviceMake();

        string GetDeviceModel();

        bool HasInternetConnection(string url);

    }
}
