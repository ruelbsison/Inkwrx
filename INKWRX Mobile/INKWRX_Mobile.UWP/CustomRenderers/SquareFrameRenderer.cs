using INKWRX_Mobile.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Frame), typeof(SquareFrameRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class SquareFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(0);
            }
        }
    }
}
