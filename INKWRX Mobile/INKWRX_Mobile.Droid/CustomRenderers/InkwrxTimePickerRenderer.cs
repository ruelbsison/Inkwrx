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

[assembly: ExportRenderer(typeof(InkwrxTimePicker), typeof(InkwrxTimePickerRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class InkwrxTimePickerRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
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

                if (!((InkwrxTimePicker)e.NewElement).IsEnabled)
                {
                    this.Element.IsEnabled = true;
                    this.Control.InputType = Android.Text.InputTypes.Null;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == InkwrxTimePicker.NullableTimeProperty.PropertyName)
            {
                this.setEmptyIfNull();
            }
        }

        private void setEmptyIfNull()
        {
            var picker = (InkwrxTimePicker)this.Element;
            if (picker != null && picker.NullableTime == null)
            {
                this.Control.Text = "";
            }
        }
    }
}