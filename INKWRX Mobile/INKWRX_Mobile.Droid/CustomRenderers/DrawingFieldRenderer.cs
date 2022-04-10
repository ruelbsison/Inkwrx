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
using INKWRX_Mobile.Droid.UI;
using Xamarin.Forms.Platform.Android;
using INKWRX_Mobile.UI;
using Xamarin.Forms;
using INKWRX_Mobile.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(DrawingFieldView), typeof(DrawingFieldRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class DrawingFieldRenderer : ViewRenderer<DrawingFieldView, DrawingView>
    {
        private DrawingView drawingView = null;
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingFieldView> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (Control == null && elementChangedEventArgs.OldElement == null)
            {
                this.drawingView = new DrawingView(this.Context, Element);
                SetNativeControl(this.drawingView);
            }

            if (elementChangedEventArgs.OldElement != null)
            {
                elementChangedEventArgs.OldElement.RequiresUpdate -= UpdateView;
            }

            if (elementChangedEventArgs.NewElement != null)
            {
                elementChangedEventArgs.NewElement.RequiresUpdate += UpdateView;
            }
        }

        private void UpdateView(object sender, EventArgs eventArgs)
        {
            drawingView.RedrawLines();
        }

        protected override void OnDetachedFromWindow()
        {
            this.Element.RequiresUpdate -= UpdateView;
        }
    }
}