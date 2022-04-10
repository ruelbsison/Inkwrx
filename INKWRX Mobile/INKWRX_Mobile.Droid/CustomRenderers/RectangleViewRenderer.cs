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
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FormTools.FormDescriptor;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(RectangleView), typeof(RectangleViewRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class RectangleViewRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                GradientDrawable gd = new GradientDrawable();
                var rect = (RectangleView)e.NewElement;
                if (rect.Descriptor is RoundedRectangleDescriptor)
                {
                    gd.SetCornerRadius((float)((RoundedRectangleDescriptor)rect.Descriptor).Radius.X);
                }
                else
                {
                    gd.SetCornerRadius(0);
                }
                gd.SetStroke(rect.Descriptor.StrokeWidth, new Android.Graphics.Color(
                    (byte)rect.Descriptor.StrokeColour.Red,
                    (byte)rect.Descriptor.StrokeColour.Green,
                    (byte)rect.Descriptor.StrokeColour.Blue));
                gd.SetColor(new Android.Graphics.Color(
                    (byte)rect.Descriptor.FillColour.Red,
                    (byte)rect.Descriptor.FillColour.Green,
                    (byte)rect.Descriptor.FillColour.Blue));
                SetBackgroundDrawable(gd);
            }
        }
    }
}