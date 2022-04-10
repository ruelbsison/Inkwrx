using FormTools.FormDescriptor;
using INKWRX_Mobile.Database;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using INKWRX_Mobile.FormService;
using INKWRX_Mobile.Connect;
using System.Net;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;

namespace INKWRX_Mobile
{
    public class App : Application
    {
        public App()
        {
            SQLitePCL.Batteries.Init();

            //MainPage = new NavigationPage(new HomePage());
            this.Login = new LoginPage();
            MainPage = this.Login;
        }

        protected override async void OnStart()
        {
            // Handle when your app starts.
            await DatabaseHelper.FixFolders();
            await DatabaseHelper.DeleteOver30Days();
			//if (Device.OnPlatform(true, false, false)) {
			//	Device.BeginInvokeOnMainThread(() =>
			//	{
			//		this.GetGps();
			//		Device.StartTimer(TimeSpan.FromSeconds(30), this.GetGps);
			//	});
			//}
            if (Device.RuntimePlatform == Device.iOS)
            {
                this.GetGps();
                Device.StartTimer(TimeSpan.FromSeconds(30), this.GetGps);
            }
        }

		public Position CurrentPosition { get; set; }

		private async void GetGPSLoc()
		{
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;

				this.CurrentPosition = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

			}
			catch (GeolocationException)
			{
				//gps is switched off, so send with empty gps
				this.CurrentPosition = this.CurrentPosition
				?? new Position
				{
					Latitude = 0,
					Longitude = 0,
					Altitude = 0,
					Timestamp = DateTimeOffset.Now
				};
			}
		}

		private bool GetGps()
		{
			
			if (DependencyService.Get<IBackgroundService>().IsInBackground())
			{
				this.CurrentPosition = this.CurrentPosition
					?? new Position
						{
							Latitude = 0,
							Longitude = 0,
							Altitude = 0,
							Timestamp = DateTimeOffset.Now
						};
			}
			else 
			{
				this.GetGPSLoc();
			}
			return true;
		}

        private bool autosavedOnSleep = false;

        protected override void OnSleep()
        {
            if(this.MainPage is FormViewPage)
            {
                ((FormViewPage)this.MainPage).Processor.StopAutosaving();
            }
        }

        public void Logout()
        {
            this.LoggingOut = true;
            if (this.MainPage is FormViewPage)
            {
                ((FormViewPage)this.MainPage).Processor.AutoSave(false,true);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await this.MainPage.DisplayAlert("Logged out", "You have been logged out due to a conflict with username/password.\nYour form has been autosaved.", "Ok");
                });
                autosavedOnSleep = true;
            }
            this.FormSendingService?.CancelSending();
            this.FormSendingService = null;
            this.LoggedInUser = null;
            this.Home = null;
            this.MainPage = Login;
            this.LoggingOut = false;
        }

        public static bool TestWeb()
        {

            return true;
        }
        public static void TestWebAccess(string uri, ref WebRequest request, Action<IAsyncResult> callBack)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)) return;
            request = WebRequest.Create(uri);
            request.BeginGetResponse(new AsyncCallback(callBack), null);

        }
        
        

        protected override async void OnResume()
        {
            if (this.MainPage is FormViewPage)
            {
                ((FormViewPage)this.MainPage).Processor.StartAutosaveTimer();
            }
        }

        private void Client_SendDataCompleted(object sender, SendDataCompletedEventArgs e)
        {
            var res = e.Error;
        }

        private static DatabaseHelper dbHelper = null;

        public static DatabaseHelper DatabaseHelper
        {
            get
            {
                if (dbHelper == null)
                {
                    dbHelper = new DatabaseHelper(DependencyService.Get<IDatabaseFileHelper>().GetLocalDatabasePath("inkwrx_mobile.db"));
                }
                return dbHelper;
            }
        }

        public bool LoggingOut { get; private set; }

        private static WebService webService = null;
        
        public static WebService WebService
        {
            get
            {
                if (webService == null)
                {
                    webService = new WebService(CoreAppTools.Servers[CoreAppTools.CurrentServer][CoreAppTools.Service.Form],
                        CoreAppTools.Servers[CoreAppTools.CurrentServer][CoreAppTools.Service.ServiceCenter]);
                }
                return webService;
            }
        }

        public User LoggedInUser = null;

        private FormSendingService formSendingService = null;

        public FormSendingService FormSendingService
        {
            get
            {
                return this.formSendingService;
            }
            set
            {
                if (this.formSendingService != null)
                {
                    this.formSendingService.CancelSending();
                }
                this.formSendingService = value;
            }
        }
        public LoginPage Login { get; set; }
        public HomePage Home { get; set; }
    }
}
