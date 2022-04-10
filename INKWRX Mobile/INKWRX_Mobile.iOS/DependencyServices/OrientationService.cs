using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(OrientationService))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class OrientationService : IOrientation
    {
        public void SetLandscape()
        {
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.Landscape;
            UIApplication.SharedApplication.SetStatusBarOrientation(UIInterfaceOrientation.LandscapeLeft, false);
        }

        public void SetPortrait()
        {
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.Portrait;
            UIApplication.SharedApplication.SetStatusBarOrientation(UIInterfaceOrientation.Portrait, false);
        }
    }
}
