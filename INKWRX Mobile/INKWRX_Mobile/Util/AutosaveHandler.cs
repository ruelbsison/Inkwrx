using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.Util
{
    public class AutosaveHandler
    {
        public AutosaveHandler(FormProcessor formProcessor)
        {
            this.FormProcessor = formProcessor;
        }

        /// <summary>
        /// Start the autosave timer
        /// </summary>
        public void StartTimer()
        {
            this.cancel = false;
            this.RunTimer();
        }

        private void RunTimer()
        {
            Device.StartTimer(TimeSpan.FromMinutes(2), TimerTicked);
        }

        /// <summary>
        /// Call-back void for Device.StartTimer
        /// </summary>
        /// <returns>Returns a bool to indicate whether to repeat the timer</returns>
        private bool TimerTicked()
        {
            if (cancel)
            {
                return false;
            }

            this.FormProcessor.AutoSave();

            return !cancel;
        }

        private bool cancel;
        public void CancelTimer()
        {
            this.cancel = true;
        }

        public FormProcessor FormProcessor { get; set; }
    }
}
