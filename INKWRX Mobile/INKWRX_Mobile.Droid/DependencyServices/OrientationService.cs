using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INKWRX_Mobile.Dependencies;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(INKWRX_Mobile.Droid.DependencyServices.OrientationService))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    public class OrientationService : IOrientation
    {
        public void SetLandscape()
        {
            
        }

        public void SetPortrait()
        {

        }
    }
}