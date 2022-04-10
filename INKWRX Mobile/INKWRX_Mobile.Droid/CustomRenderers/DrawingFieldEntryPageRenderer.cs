using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using INKWRX_Mobile.Views;
using INKWRX_Mobile.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(DrawingFieldEntryPage), typeof(DrawingFieldEntryPageRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    public class DrawingFieldEntryPageRenderer : PageRenderer
    {
        protected override void OnWindowVisibilityChanged([GeneratedEnum] ViewStates visibility)
        {
            base.OnWindowVisibilityChanged(visibility);
            var activity = (Activity)Context;
            if (Visibility == ViewStates.Gone)
            {
                activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            }
            else if (Visibility == ViewStates.Visible)
            {
                activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            }
        }

        protected override void OnDetachedFromWindow()
        {
            ((Activity)Context).RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
        }
    }
}