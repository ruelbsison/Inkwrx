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
using Xamarin.Forms;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Droid.CustomRenderers;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(ISOFieldView), typeof(ISOFieldRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    public class ISOFieldRenderer : EntryRenderer
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
                shape.Paint.Color = ((ISOFieldView)e.NewElement).RawDescriptor.StrokeColour.ToColor().ToAndroid();
                shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                Control.Background = shape;
                this.Control.SetPadding(3, 2, 2, 2);

                if (! ((ISOFieldView)e.NewElement).IsEnabled)
                {
                    this.Element.IsEnabled = true;
                    this.Control.InputType = Android.Text.InputTypes.Null;
                }
                else if (((ISOFieldView)e.NewElement).CapsOnly)
                {
                    this.Control.InputType = this.Control.InputType | Android.Text.InputTypes.TextFlagCapCharacters;
                }
            }
        }

        private void ControlChanged(object sender, Android.Text.AfterTextChangedEventArgs eventArgs)
        {
            var thisIso = (ISOFieldView)Element;
            var oldText = thisIso.Text;
            if (thisIso.Changing) return;
            var requiresChange = false;
            var newText = thisIso.Text ?? "";
            var selectedRange = Control.SelectionStart;
            if (thisIso.CapsOnly)
            {
                if (newText.ContainsCharacter("abcdefghijklmnopqrstuvwxyz"))
                {
                    newText = newText.ToUpper();
                    requiresChange = true;
                }
            }

            if (!thisIso.AllowsNumber && newText.Length > 0)
            {
                var notAllowed = "0123456789";
                if (newText.ContainsCharacter(notAllowed))
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }
            }

            if (!thisIso.AllowsText && newText.Length > 0)
            {
                bool allow;
                if (thisIso.Descriptor.IsCalcField)
                {
                    allow = newText.IsOnlyCharacters("0123456789.-#");
                }
                else
                {
                    allow = newText.IsOnlyCharacters("0123456789.-");
                }
                if (!allow)
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }
            }

            if (!thisIso.FdtFormat.Contains("sym") && newText.Length > 0)
            {
                if (thisIso.Descriptor.IsCalcField)
                {
                    if (newText.ContainsCharacter("+=_:;?/|\\!\"£$%^&*()-"))
                    {
                        requiresChange = true;
                        newText = thisIso.OldText;
                    }
                }
                else if (newText.ContainsCharacter("+=_:;#?/|\\!\"£$%^&*()-"))
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }
            }

            if (requiresChange)
            {

                var change = oldText.Length - newText.Length;
                if (selectedRange + change < 0)
                {
                    change = 0;
                }

                thisIso.Changing = true;
                thisIso.Text = newText;
                thisIso.Changing = false;

                try
                {
                    Control.SetSelection(selectedRange - change);
                }
                catch (Java.Lang.IndexOutOfBoundsException)
                {

                }

                thisIso.OldText = newText;
            }
            else
            {
                thisIso.OldText = thisIso.Text;
            }
        }
    }
}