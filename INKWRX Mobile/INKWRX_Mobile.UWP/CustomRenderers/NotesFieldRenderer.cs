using INKWRX_Mobile.UI;
using INKWRX_Mobile.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NotesFieldView), typeof(NotesFieldRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class NotesFieldRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
            {
                var notes = (NotesFieldView)e.NewElement;
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                Control.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255,
                    (byte)notes.Descriptor.StrokeColour.Red,
                    (byte)notes.Descriptor.StrokeColour.Green,
                    (byte)notes.Descriptor.StrokeColour.Blue));
                this.Control.TextChanged += ControlChanged;
            }
            if (Control != null && e.OldElement != null)
            {
                Control.TextChanged -= ControlChanged;
            }
        }

        private bool changing = false;
        private string oldText = "";

        private void ControlChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs eventArgs)
        {
            if (this.changing)
            {
                return;
            }
            this.changing = true;

            var selectedRange = Control.SelectionStart;
            var newText = this.Control.Text ?? "";

            var newLines = newText.Length - newText.Replace("\r", "").Length;
            if (newLines > ((NotesFieldView)Element).Descriptor.RectElements.Count -1) // -1 for the default first line
            {
                this.Control.Text = oldText;
                this.Control.SelectionStart = selectedRange;
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
