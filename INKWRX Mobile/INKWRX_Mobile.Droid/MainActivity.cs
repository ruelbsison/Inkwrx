using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Plugin.Toasts;

namespace INKWRX_Mobile.Droid
{
    [Activity(Label = "INKWRX_Mobile", Icon = "@drawable/iw_app_icon_application_mobile", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init(this);

            this.Window.SetStatusBarColor(new Android.Graphics.Color(
                (int)(CoreAppTools.SteelBlue.R * 255d),
                (int)(CoreAppTools.SteelBlue.G * 255d),
                (int)(CoreAppTools.SteelBlue.B * 255d)));

            LoadApplication(new App());

            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
        }

        //validate certificate chain, returns whether the certificate is valid
        private static bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((System.Security.Cryptography.X509Certificates.X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}

