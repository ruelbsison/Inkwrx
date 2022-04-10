using Foundation;
using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(NotesFieldView), typeof(NotesFieldRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class NotesFieldRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
            {
                var notes = (NotesFieldView)e.NewElement;
                Control.Layer.BorderWidth = 1;
                Control.Layer.BorderColor = notes.Descriptor.StrokeColour.ToColor().ToCGColor();
                Control.ShouldChangeText = this.ShouldChangeText;     
            }
            else
            {

            }
        }

        public bool ShouldChangeText(UITextView textView, NSRange range, string replacementText)
        {
            if (string.IsNullOrEmpty(replacementText))
            {
                return true;
            }

            var notes = (NotesFieldView)Element;
            var lineArray = textView.Text.Split('\n');

            var newLines = lineArray.Length;
            foreach (string line in lineArray)
            {
                newLines += (int)Math.Floor(line.Length / (double)((NotesFieldView)Element).LimitPerLine);//calculate additional lines by wrapping
            }

            
            if (newLines > notes.Descriptor.RectElements.Count) 
            {
                return false;
            }

			if (replacementText == "\n" && newLines > notes.Descriptor.RectElements.Count - 1)
			{
				return false;
			}
            

            if (textView.Text.Length + replacementText.Length - newLines > notes.CharLimit)
            {
                return false;
            }

            return true;
        }
    }
}
