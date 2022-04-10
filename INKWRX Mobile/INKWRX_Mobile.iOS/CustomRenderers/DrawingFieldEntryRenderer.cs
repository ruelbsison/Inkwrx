using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.iOS.UI;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawingFieldEntryView), typeof(DrawingFieldEntryRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class DrawingFieldEntryRenderer : ViewRenderer<DrawingFieldEntryView, DrawingEntryView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingFieldEntryView> e)
        {
            base.OnElementChanged(e);
            if (Control == null && e.OldElement == null)
            {
                drawingView = new DrawingEntryView(Element);
                SetNativeControl(drawingView);
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

        private DrawingEntryView drawingView;

        private void UpdateView(object sender, EventArgs eventArgs)
        {
            drawingView.RedrawLines();
        }

    }
}
