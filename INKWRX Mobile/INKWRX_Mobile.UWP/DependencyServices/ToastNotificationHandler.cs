using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UWP.DependencyServices;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly:Xamarin.Forms.Dependency(typeof(ToastNotificationHandler))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class ToastNotificationHandler : IToastNotification
    {
        public async void SendToast(string title, string message)
        {
            var notifier = DependencyService.Get<IToastNotificator>();
            var options = new NotificationOptions
            {
                Title = title,
                Description = message,
                IsClickable = true
            };
            await notifier.Notify(options);
        }
    }
}
