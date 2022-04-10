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
using Android.Graphics;
using FormTools.FormDescriptor.Label;

[assembly: ExportRenderer(typeof(TextLabelView), typeof(TextLabelViewRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class TextLabelViewRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null && e.NewElement != null)
            {
                var labelView = (TextLabelView)e.NewElement;
                if (labelView.Underline)
                {
                    Control.PaintFlags = PaintFlags.UnderlineText;
                }

                LabelSection section = labelView.Descriptor.BaseSection;
                Control.TextSize = (float) section.TextSize;
                
                string font = section.FontName.ToLower().Replace(" ", "").Replace(",", "");
                if (labelView.Descriptor.BaseSection.Bold)
                {
                    font += "-Bold";
                }
                Control.Typeface = Typeface.CreateFromAsset(Context.Assets, font + ".ttf");
            }
        }
    }
}