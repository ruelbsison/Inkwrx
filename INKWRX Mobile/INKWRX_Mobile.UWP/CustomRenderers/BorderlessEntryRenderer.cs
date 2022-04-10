using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INKWRX_Mobile.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using INKWRX_Mobile.UWP.CustomRenderers;

[assembly: ExportRenderer(typeof(BorderlessEntryView), typeof(BorderlessEntryRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(0);
                Control.IsSpellCheckEnabled = false;
                Control.Padding = new Windows.UI.Xaml.Thickness(5,10,5,0);
            }
        }
    }
}
