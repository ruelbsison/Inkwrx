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
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Droid.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(DropDownFieldView), typeof(DropdownFieldRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class DropdownFieldRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null && e.NewElement != null)
            {
                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                shape.Paint.Color = ((DropDownFieldView)e.NewElement).RawDescriptor.StrokeColour.ToColor().ToAndroid();
                shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                Control.Background = shape;

                Control.Hint = "Please Select";
            }
        }
    }
}