using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ISOFieldView), typeof(ISOFieldRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class ISOFieldRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null && Control != null)
            {
                Control.EditingChanged -= ControlChanged;
            }
            if (e.NewElement != null)
            {
                Control.EditingChanged += ControlChanged;
				if (((ISOFieldView)e.NewElement).FdtFormat.ToLower().Contains("uppercase")
					&& !((ISOFieldView)e.NewElement).FdtFormat.ToLower().Contains("lowercase"))
				{
					Control.AutocapitalizationType = UIKit.UITextAutocapitalizationType.AllCharacters;
				}
            }
        }
        

        private void ControlChanged(object sender, EventArgs eventArgs)
        {
            var thisIso = (ISOFieldView)Element;
            var oldText = thisIso.Text;
            if (thisIso.Changing) return;
            var requiresChange = false;
            var newText = thisIso.Text ?? "";
            var selectedRange = Control.SelectedTextRange;
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
                var allow = false;
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
                if (newText.ContainsCharacter("+=_:;#?/|\\!\"£$%^&*()-"))
                {
                    requiresChange = true;
                    newText = thisIso.OldText;
                }
            }

            if (requiresChange)
            {

                var change = -1 * (oldText.Length - newText.Length);
                var newPosition = Control.GetPosition(selectedRange.Start, (nint)change);

                thisIso.Changing = true;
                thisIso.Text = newText;
                thisIso.Changing = false;
                if (newPosition != null) // before we fail miserably
                {
                    Control.SelectedTextRange = Control.GetTextRange(newPosition, newPosition);
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
