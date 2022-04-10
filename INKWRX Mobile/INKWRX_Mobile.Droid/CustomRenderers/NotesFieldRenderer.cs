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

[assembly: ExportRenderer(typeof(NotesFieldView), typeof(NotesFieldRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class NotesFieldRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
            {
                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RoundRectShape(new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }, null, null));
                shape.Paint.Color = ((NotesFieldView)e.NewElement).RawDescriptor.StrokeColour.ToColor().ToAndroid();
                shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                Control.Background = shape;

                if (!((NotesFieldView)e.NewElement).IsEnabled)
                {
                    this.Element.IsEnabled = true;
                    this.Control.InputType = Android.Text.InputTypes.Null;
                }

                this.Control.SetPadding(1, 1, 1, 1);

                this.Control.AfterTextChanged += ControlChanged;
            }
            if (Control != null && e.OldElement != null)
            {
                Control.AfterTextChanged -= ControlChanged;
            }
        }

        private bool changing = false;
        private string oldText = "";

        private void ControlChanged(object sender, Android.Text.AfterTextChangedEventArgs eventArgs)
        {
            if (this.changing)
            {
                return;
            }
            this.changing = true;

            var selectedRange = Control.SelectionStart;
            if(selectedRange > 0)
            {
                selectedRange --;
            }
            var newText = this.Control.Text ?? "";

            string[] lineArray = newText.Split('\n');
            var newLines = lineArray.Count();
            foreach (string line in lineArray)
            {
                newLines += line.Length / ((NotesFieldView)Element).LimitPerLine;//calculate additional lines by wrapping
            }
            if (newLines > ((NotesFieldView)Element).Descriptor.RectElements.Count)
            {
                this.Control.Text = oldText;
                Control.SetSelection(selectedRange);
                this.changing = false;
                return;
            }

            if (newText.Length - newLines > ((NotesFieldView)Element).CharLimit)
            {
                this.Control.Text = oldText;
                this.changing = false;
                return;
            }

            this.oldText = this.Control.Text ?? "";
            this.changing = false;

        }
    }
}