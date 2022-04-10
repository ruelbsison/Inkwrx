using FormTools.FormDescriptor;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly:ExportRenderer(typeof(RectangleView), typeof(RectangleViewRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class RectangleViewRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
            {
                var rect = (RectangleView)e.NewElement;
                if (rect.Descriptor is RoundedRectangleDescriptor)
                {
                    Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(((RoundedRectangleDescriptor)rect.Descriptor).Radius.X);
                        
                }
                else
                {
                    Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(0);
                }
                Control.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 
                    (byte)rect.Descriptor.StrokeColour.Red, 
                    (byte)rect.Descriptor.StrokeColour.Green, 
                    (byte)rect.Descriptor.StrokeColour.Blue));
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(rect.Descriptor.StrokeWidth);
            }
        }
    }
}
