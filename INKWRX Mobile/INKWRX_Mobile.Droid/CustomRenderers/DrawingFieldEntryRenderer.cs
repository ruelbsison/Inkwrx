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
using Xamarin.Forms.Platform.Android;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Droid.UI;
using Xamarin.Forms;
using INKWRX_Mobile.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(DrawingFieldEntryView), typeof(DrawingFieldEntryRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class DrawingFieldEntryRenderer : ViewRenderer<DrawingFieldEntryView, DrawingEntryView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DrawingFieldEntryView> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);
            if (Control == null && elementChangedEventArgs.OldElement == null)
            {
                drawingView = new DrawingEntryView(this.Context, Element);
                
                SetNativeControl(drawingView);
                drawingView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
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

        private DrawingEntryView drawingView;

        private void UpdateView(object sender, EventArgs eventArgs)
        {
            drawingView.RedrawLines();
        }
    }
}