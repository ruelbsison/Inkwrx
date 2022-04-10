using INKWRX_Mobile.UI;
using INKWRX_Mobile.UWP.CustomRenderers;
using INKWRX_Mobile.UWP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(DrawingFieldEntryView), typeof(DrawingFieldEntryRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class DrawingFieldEntryRenderer : ViewRenderer<DrawingFieldEntryView, DrawingEntryView>
    {
        private DrawingEntryView drawingView = null;
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

        private void UpdateView(object sender, EventArgs eventArgs)
        {
            if (drawingView != null)
            {
                drawingView.RedrawLines();
            }
        }
    }
}
