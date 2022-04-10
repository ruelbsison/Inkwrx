using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MessageBar;

[assembly:Xamarin.Forms.Dependency(typeof(ToastNotificationHandler))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class ToastNotificationHandler : IToastNotification
    {
        public void SendToast(string title, string message)
        {
			Device.BeginInvokeOnMainThread(() =>
			{
				var type = title.ToLower().Contains("error") || message.ToLower().Contains("error")
							? MessageType.Error
							: MessageType.Success;
				MessageBarManager.SharedInstance.ShowMessage(title, message, type);
			});

        }
    }
}
