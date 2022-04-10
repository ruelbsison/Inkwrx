using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(BorderlessEntryView), typeof(BorderlessEntryViewRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    public class BorderlessEntryViewRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.OldElement == null)
            {
                Control.BorderStyle = UIKit.UITextBorderStyle.None;
            }
        }
    }
}
