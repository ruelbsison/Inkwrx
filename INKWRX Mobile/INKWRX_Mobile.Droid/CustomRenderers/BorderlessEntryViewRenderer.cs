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

[assembly: ExportRenderer(typeof(BorderlessEntryView), typeof(BorderlessEntryViewRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class BorderlessEntryViewRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.OldElement == null)
            {
                Control.Background = null;
            }
        }
    }
}