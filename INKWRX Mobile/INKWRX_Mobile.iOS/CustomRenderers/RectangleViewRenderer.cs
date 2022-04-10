using FormTools.FormDescriptor;
using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RectangleView), typeof(RectangleViewRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class RectangleViewRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
           
            if (e.NewElement != null)
            {
                var rect = (RectangleView)e.NewElement;
                if (rect.Descriptor is RoundedRectangleDescriptor)
                {
                    this.Layer.CornerRadius = (nfloat)((RoundedRectangleDescriptor)rect.Descriptor).Radius.X;
                }
                else
                {
                    this.Layer.CornerRadius = 0;
                }
                this.Layer.BorderColor = UIColor.FromRGB((byte)rect.Descriptor.StrokeColour.Red,
                    (byte)rect.Descriptor.StrokeColour.Green,
                    (byte)rect.Descriptor.StrokeColour.Blue).CGColor;
                this.Layer.BorderWidth = rect.Descriptor.StrokeWidth;
                
            }
        }
    }
}
