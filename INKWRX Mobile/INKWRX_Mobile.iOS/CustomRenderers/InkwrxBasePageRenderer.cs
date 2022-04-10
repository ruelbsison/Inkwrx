using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(InkwrxBasePage), typeof(InkwrxBasePageRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class InkwrxBasePageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.Portrait;
            UIApplication.SharedApplication.SetStatusBarOrientation(UIInterfaceOrientation.Portrait, false);
        }

        public override UIKit.UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIKit.UIInterfaceOrientationMask.Portrait;
        }

        public override bool ShouldAutorotate()
        {
            return false;
        }

        public override UIKit.UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            return UIKit.UIInterfaceOrientation.Portrait;
        }
    }
}
