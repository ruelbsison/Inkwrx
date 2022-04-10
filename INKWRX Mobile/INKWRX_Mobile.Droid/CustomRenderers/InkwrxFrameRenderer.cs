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
using Xamarin.Forms;
using INKWRX_Mobile.Droid.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(Frame), typeof(InkwrxFrameRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class InkwrxFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                ShapeDrawable backgroundColour = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                backgroundColour.Paint.Color = e.NewElement.BackgroundColor.ToAndroid();
                backgroundColour.Paint.SetStyle(Android.Graphics.Paint.Style.Fill);
                ShapeDrawable borderColour = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                borderColour.Paint.Color = e.NewElement.OutlineColor.ToAndroid();
                borderColour.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                this.Background = new LayerDrawable(new Drawable[] { backgroundColour, borderColour });
            }
        }
    }
}