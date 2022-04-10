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
using INKWRX_Mobile.Droid.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using INKWRX_Mobile.Views.UI;

[assembly: ExportRenderer(typeof(FormListItemView), typeof(InkwrxViewCellRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class InkwrxViewCellRenderer : ViewCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            Android.Views.View cellCore = base.GetCellCore(item, convertView, parent, context);

            cellCore.SetBackgroundColor(Color.White.ToAndroid());

            return cellCore;
        }
    }
}