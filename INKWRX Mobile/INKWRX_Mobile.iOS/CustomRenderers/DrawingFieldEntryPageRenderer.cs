using System;
using INKWRX_Mobile.iOS;
using INKWRX_Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawingFieldEntryPage), typeof(DrawingFieldEntryPageRenderer))]
namespace INKWRX_Mobile.iOS
{
	public class DrawingFieldEntryPageRenderer : PageRenderer
	{
		public override UIKit.UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIKit.UIInterfaceOrientationMask.LandscapeRight;
            
		}

		public override bool ShouldAutorotate()
		{
			return false;
		}

		public override UIKit.UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
		{
			return UIKit.UIInterfaceOrientation.LandscapeRight;
		}
	}
}
