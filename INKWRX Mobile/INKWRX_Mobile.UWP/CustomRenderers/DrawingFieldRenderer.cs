using INKWRX_Mobile.UI;
using INKWRX_Mobile.UWP.CustomRenderers;
using INKWRX_Mobile.UWP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(DrawingFieldView), typeof(DrawingFieldRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class DrawingFieldRenderer : ViewRenderer<DrawingFieldView, DrawingView>
    {
        private DrawingView drawingView = null;
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingFieldView> e)
        {
            base.OnElementChanged(e);
            if (Control == null && e.OldElement == null)
            {
                drawingView = new DrawingView(Element);
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
