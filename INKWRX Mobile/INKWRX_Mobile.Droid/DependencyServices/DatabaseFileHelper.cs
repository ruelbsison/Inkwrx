using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INKWRX_Mobile.Dependencies;
using System.IO;
using INKWRX_Mobile.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseFileHelper))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    public class DatabaseFileHelper : IDatabaseFileHelper
    {
        public string GetLocalDatabasePath(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, fileName);
        }
    }
}