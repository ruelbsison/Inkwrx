using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly:ExportRenderer(typeof(CameraPage), typeof(CameraPageRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class CameraPageRenderer : PageRenderer
    {
        public UIInterfaceOrientation CurrentOrientation { get; set; }



        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            this.CurrentOrientation = toInterfaceOrientation;
            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    ((CameraPage)Element).CurrentOrientation = CameraPage.Orientation.LandscapeLeft;
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    ((CameraPage)Element).CurrentOrientation = CameraPage.Orientation.LandscapeRight;
                    break;
                default:
                    ((CameraPage)Element).CurrentOrientation = CameraPage.Orientation.Portrait;
                    break;
            }
            return base.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
        }
    }
}
