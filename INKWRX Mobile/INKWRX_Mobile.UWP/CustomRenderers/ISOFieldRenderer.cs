using INKWRX_Mobile.UI;
using INKWRX_Mobile.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ISOFieldView), typeof(ISOFieldRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class ISOFieldRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null && Control != null)
            {

                Control.TextChanged -= ControlChanged;

            }
            if (e.NewElement != null && Control != null)
            {

                Control.TextChanged += ControlChanged;


            }
        }


        private void ControlChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs eventArgs)
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
                var allow = false;
                if (thisIso.Descriptor.IsCalcField)
                {
                    allow = newText.IsOnlyCharacters("0123456789.-#");
                }
                else
                {
                    allow = newText.IsOnlyCharacters("0123456789-");
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
                if (selectedRange + change < 0)
                {
                    change = 0;
                }

                thisIso.Changing = true;
                thisIso.Text = newText;
                thisIso.Changing = false;

                Control.SelectionStart = selectedRange + change;
                
                thisIso.OldText = newText;
            }
            else
            {
                thisIso.OldText = thisIso.Text;
            }
        }

    }
}
