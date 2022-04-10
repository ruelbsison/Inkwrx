using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.iOS.UI;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawingFieldView), typeof(DrawingFieldRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class DrawingFieldRenderer : ViewRenderer<DrawingFieldView, DrawingView>
    {
        private DrawingView drawingView = null;
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingFieldView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.OldElement == null)
            {
                this.drawingView = new DrawingView(Element);
                SetNativeControl(this.drawingView);
            }

            if (e.OldElement != null)
            {
                e.OldElement.RequiresUpdate -= UpdateView;
            }

            if (e.NewElement != null)
            {
                e.NewElement.RequiresUpdate += UpdateView;
            }
        }

        private void UpdateView(object sender, EventArgs eventArgs)
        {
            drawingView.RedrawLines();
        }

    }
}
