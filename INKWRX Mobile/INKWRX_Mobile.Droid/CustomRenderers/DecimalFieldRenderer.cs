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
using System.Globalization;

[assembly: ExportRenderer(typeof(DecimalFieldView), typeof(DecimalFieldRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class DecimalFieldRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null && Control != null)
            {
                Control.AfterTextChanged -= ControlChanged;
            }
            if (e.NewElement != null && Control != null)
            {
                Control.AfterTextChanged += ControlChanged;

                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                shape.Paint.Color = ((DecimalFieldView)e.NewElement).RawDescriptor.StrokeColour.ToColor().ToAndroid();
                shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                Control.Background = shape;

                if (!((DecimalFieldView)e.NewElement).IsEnabled)
                {
                    this.Element.IsEnabled = true;
                    this.Control.InputType = Android.Text.InputTypes.Null;
                }
            }
        }

        private void ControlChanged(object sender, Android.Text.AfterTextChangedEventArgs eventArgs)
        {
            var thisIso = (DecimalFieldView)Element;
            var testText = thisIso.Text ?? "";
            if (this.Changing) return;
            var requiresChange = false;
            var newText = testText;
            var selectedRange = Control.SelectionStart;


            //first check allowed characters
            var allow = false;
            if (thisIso.Descriptor.IsCalcField)
            {
                allow = newText.IsOnlyCharacters("0123456789.-#");
            }
            else
            {
                if (thisIso.Descriptor.FdtListArray.Contains("|"))
                {
                    allow = newText.IsOnlyCharacters("0123456789.-");
                }
                else
                {
                    allow = newText.IsOnlyCharacters("0123456789-");
                }
            }
            if (!allow)
            {
                requiresChange = true;
                newText = thisIso.OldText;
            }
            if (allow)
            {

                // if being updated as part of a calc field, allow # as this is from the app
                if (newText.Contains("#"))
                {
                    thisIso.OldText = newText;
                    return;
                }

                //fix decimal at the start
                if (newText.StartsWith("."))
                {
                    requiresChange = true;
                    newText = "0" + newText;
                }

                // make sure there are not 2 decimal points...
                if (newText.Length - newText.Replace(".", "").Length > 1)
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }

                // get the double value of the entered text
                var doubleValue = 0d;
                if (newText.EndsWith("."))
                {
                    double.TryParse(newText.Substring(0, newText.Length - 1), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleValue);
                }
                else
                {
                    double.TryParse(newText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleValue);
                }

                //create the maximum value, based on fdtListArray
                var maxStringBuilder = new StringBuilder();
                var intCount = 0;
                var decimalCount = 0;
                if (thisIso.Descriptor.FdtListArray.Contains("|")) // for decimal fields
                {
                    var split = thisIso.Descriptor.FdtListArray.Split('|');
                    intCount = int.Parse(split[0]);
                    decimalCount = int.Parse(split[1]);
                }
                else // for number fields (integers)
                {
                    intCount = int.Parse(thisIso.Descriptor.FdtListArray);
                }


                // build up the string
                for (var intCurrent = 0; intCurrent < intCount; intCurrent++)
                {
                    maxStringBuilder.Append("9");
                }

                if (decimalCount > 0)
                {
                    maxStringBuilder.Append(".");
                    for (var decimalCurrent = 0; decimalCurrent < decimalCount; decimalCurrent++)
                    {
                        maxStringBuilder.Append("9");
                    }
                }

                // convert max string to max double
                var maxDouble = double.Parse(maxStringBuilder.ToString(), CultureInfo.InvariantCulture);

                // check if value is less than or equal to max double
                if (doubleValue > maxDouble)
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }

                // check decimal places
                if (newText.Contains("."))
                {
                    // get string after "." and to end
                    var decimals = newText.Substring(newText.IndexOf(".") + 1);
                    // make sure we have the right number of decimal places
                    if (decimals.Length > decimalCount)
                    {
                        requiresChange = true;
                        newText = thisIso.OldText;
                    }
                }
            }
            // make the changes as required
            if (requiresChange)
            {
                // set up the changed location, as per ISO field renderer
                var change = -1 * (testText.Length - newText.Length);
                if (selectedRange + change < 0)
                {
                    change = 0;
                }

                this.Changing = true;
                thisIso.Text = newText;
                Control.SetSelection(selectedRange + change);
            }

            thisIso.OldText = thisIso.Text ?? "";
            this.Changing = false;
        }

        public bool Changing { get; private set; }
    }
}