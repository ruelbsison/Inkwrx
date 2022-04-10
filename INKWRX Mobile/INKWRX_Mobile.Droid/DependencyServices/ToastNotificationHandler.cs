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
using INKWRX_Mobile.Droid.DependencyServices;
using Plugin.Toasts;
using Xamarin.Forms;
using INKWRX_Mobile.Dependencies;

[assembly:Xamarin.Forms.Dependency(typeof(ToastNotificationHandler))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    public class ToastNotificationHandler : IToastNotification
    {
        public async void SendToast(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Android.App.Application.Context, title + "\n" + message, ToastLength.Long).Show();
            });
        }
    }
}