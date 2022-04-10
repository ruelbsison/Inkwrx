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

[assembly: ExportRenderer(typeof(InkwrxDatePicker), typeof(InkwrxDatePickerRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class InkwrxDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null && e.NewElement != null)
            {
                this.setEmptyIfNull();

                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                shape.Paint.Color = Android.Graphics.Color.Black;//TODO at some point might need to implemenet colour from descriptor
                shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                Control.Background = shape;
                Control.HintFormatted = new Java.Lang.String(e.NewElement.Format.ToUpper());

                if (!((InkwrxDatePicker)e.NewElement).IsEnabled)
                {
                    this.Element.IsEnabled = true;
                    this.Control.Clickable = false;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == InkwrxDatePicker.NullableDateProperty.PropertyName)
            {
                this.setEmptyIfNull();
            }
        }

        private void setEmptyIfNull()
        {
            var picker = (InkwrxDatePicker)this.Element;
            if (picker != null && picker.NullableDate == null)
            {
                this.Control.Text = "";
            }
        }
    }
}