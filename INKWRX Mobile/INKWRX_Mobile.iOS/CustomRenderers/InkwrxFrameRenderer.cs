using INKWRX_Mobile.iOS.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(Frame), typeof(InkwrxFrameRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class InkwrxFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                //this.Layer.CornerRadius = 0;
            }
        }
    }
}
