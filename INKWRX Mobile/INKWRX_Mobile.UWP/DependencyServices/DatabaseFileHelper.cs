using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseFileHelper))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class DatabaseFileHelper : IDatabaseFileHelper
    {
        public string GetLocalDatabasePath(string fileName)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
        }
    }
}
