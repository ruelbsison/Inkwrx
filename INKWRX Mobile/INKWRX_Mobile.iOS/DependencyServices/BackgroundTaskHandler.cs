using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

[assembly:Xamarin.Forms.Dependency(typeof(BackgroundTaskHandler))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class BackgroundTaskHandler : IBackgroundService
    {
        private nint backgroundTask;
		private bool taskRunning = false;
        public void EndBackgroundTask()
        {
            UIApplication.SharedApplication.EndBackgroundTask(backgroundTask);
			this.taskRunning = false;
        }

		public bool IsInBackground()
		{
			var background = UIApplication.SharedApplication.ApplicationState;
			return background != UIApplicationState.Active;
		}

		public int RegisterBackgroundTask()
        {
			if (this.taskRunning)
			{
				return (int)this.backgroundTask;
			}
            this.backgroundTask = UIApplication.SharedApplication.BeginBackgroundTask(() => {
				this.EndBackgroundTask();
				this.taskRunning = false;
			});
			this.taskRunning = true;
            return (int)this.backgroundTask;
        }
    }
}
