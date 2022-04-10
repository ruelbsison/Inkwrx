using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INKWRX_Mobile.Dependencies;
using System.IO;
using INKWRX_Mobile.Droid.DependencyServices;
using Xamarin.Forms;

[assembly:Xamarin.Forms.Dependency(typeof(FormFileTools))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    class FormFileTools : IFormFileTools
    {
        public void DeleteFormData(string formId, string username)
        {
            Directory.Delete(GetFormPath(formId, username), true);
        }

        public async Task<string> GetFormData(string formId, string username)
        {
            string formDataPath = Path.Combine(GetFormPath(formId, username), "formdata.txt");
            if (! File.Exists(formDataPath))
            {
                return "";
            }
            return File.ReadAllText(formDataPath);
        }

        public async Task<byte[]> GetImageData(string imageId, string formId, string username)
        {
            string imageFileName = string.Format("image{0}.jpg", imageId);
            string imagePath = Path.Combine(GetFormPath(formId, username), imageFileName);
            if (! File.Exists(imagePath))
            {
                return new byte[0];
            }
            return File.ReadAllBytes(imagePath);
        }

        public async Task<string> GetLexiconData(string lexiconId, string formId, string username)
        {
            string lexiconFileName = string.Format("lexicon{0}.txt", lexiconId);
            string lexiconPath = Path.Combine(GetFormPath(formId, username), lexiconFileName);
            if (! File.Exists(lexiconPath))
            {
                return "";
            }
            return File.ReadAllText(lexiconPath);
        }

        public void SaveAndUnzipFormFiles(string formId, string username, byte[] zipData)
        {
            string formPath = GetFormPath(formId, username);
            using (MemoryStream ms = new MemoryStream(zipData))
            {
                using (Java.Util.Zip.ZipInputStream zIStream = new Java.Util.Zip.ZipInputStream(ms))
                {
                    Java.Util.Zip.ZipEntry zipEntry;
                    byte[] buffer = new byte[1024];
                    int count;
                    string filePath;

                    while ((zipEntry = zIStream.NextEntry) != null)
                    {
                        filePath = Path.Combine(formPath, zipEntry.Name);

                        if (zipEntry.IsDirectory)
                        {
                            var fmd = new Java.IO.File(filePath);
                            fmd.Mkdirs();

                            zIStream.CloseEntry();
                            continue;
                        }

                        using (var fout = new Java.IO.FileOutputStream(filePath))
                        {
                            while ((count = zIStream.Read(buffer, 0, 1024)) != -1)
                            {
                                fout.Write(buffer, 0, count);
                            }
                        }

                        zIStream.CloseEntry();
                    }
                }
            }
        }

        private string GetFormPath(string formId, string username)
        {
            string userPath = GetUserPath(username);
            string formsDirectory = Path.Combine(userPath, "Forms");
            if (!Directory.Exists(formsDirectory))
            {
                Directory.CreateDirectory(formsDirectory);
            }
            string formDirectory = Path.Combine(formsDirectory, formId);
            if (!Directory.Exists(formDirectory))
            {
                Directory.CreateDirectory(formDirectory);
            }
            return formDirectory;
        }

        private string GetUserPath(string username)
        {
            string usernameDirectory = Path.Combine(GetAppPath(), username);
            if (!Directory.Exists(usernameDirectory))
            {
                Directory.CreateDirectory(usernameDirectory);
            }
            return usernameDirectory;
        }

        public static string GetAppPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            //return global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Android/data/com.destiny.inkwrxmobile/files";//for testing so that files can be accessed
        }
    }
}