using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly:Xamarin.Forms.Dependency(typeof(DatabaseFileHelper))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class DatabaseFileHelper : IDatabaseFileHelper
    {
        public string GetLocalDatabasePath(string fileName)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, fileName);
        }
    }
}
