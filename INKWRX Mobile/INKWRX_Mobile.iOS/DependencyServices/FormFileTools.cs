using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

[assembly:Xamarin.Forms.Dependency(typeof(FormFileTools))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class FormFileTools : IFormFileTools
    {
        public void DeleteFormData(string formId, string username)
        {
            var formFolder = this.GetFormFolder(formId, username);
            Directory.Delete(formFolder, true);
        }

        public async Task<string> GetFormData(string formId, string username)
        {
            return await Task.Run(() =>
            {
                var formFolder = this.GetFormFolder(formId, username);
                var formDataFile = Path.Combine(formFolder, "formdata.txt");
                if (!File.Exists(formDataFile))
                {
                    return "";
                }
                return File.ReadAllText(formDataFile);
            }).ConfigureAwait(true);
            
        }

        public async Task<byte[]> GetImageData(string imageId, string formId, string username)
        {
            return await Task.Run(() =>
            {
                var imageName = string.Format("image{0}.jpg", imageId);
                var formFolder = this.GetFormFolder(formId, username);
                var imagePath = Path.Combine(formFolder, imageName);
                if (!File.Exists(imagePath))
                {
                    return new byte[0];
                }
                return File.ReadAllBytes(imagePath);
            }).ConfigureAwait(true);
        }

        public async Task<string> GetLexiconData(string lexiconId, string formId, string username)
        {
            return await Task.Run(() =>
            {
                var lexiconFileName = string.Format("lexicon{0}.txt", lexiconId);
                var formFolder = this.GetFormFolder(formId, username);
                var lexiconFile = Path.Combine(formFolder, lexiconFileName);
                if (!File.Exists(lexiconFile))
                {
                    return "";
                }
                return File.ReadAllText(lexiconFile);
            }).ConfigureAwait(true);
            
        }

        public async void SaveAndUnzipFormFiles(string formId, string username, byte[] zipData)
        {
            var formFolder = this.GetFormFolder(formId, username, true);
            try
            {
                using (var stream = new MemoryStream(zipData))
                {
                    using (var zipFile = new ZipArchive(stream))
                    {
                        foreach (var entry in zipFile.Entries)
                        {
                            using (var entryfile = entry.Open())
                            {
                                var newfile = Path.Combine(formFolder, entry.Name);
                                if (File.Exists(newfile))
                                {
                                    File.Delete(newfile);
                                }
                                //File.Create(newfile);

                                using (var fileStream = new FileStream(newfile, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    await entryfile.CopyToAsync(fileStream);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
            
        }

        private string GetUserFolder(string username)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var library = Path.Combine(documents, "..", "Library");
            var dataFolder = Path.Combine(library, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            var userFolder = Path.Combine(dataFolder, username);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }
            return userFolder;
        }

        private string GetFormFolder(string formId, string username, bool replace = false)
        {
            var userFolder = this.GetUserFolder(username);
            var formFolder = Path.Combine(userFolder, formId);
            if (replace && Directory.Exists(formFolder)) {
                Directory.Delete(formFolder, true);
            }
            if (!Directory.Exists(formFolder))
            {
                Directory.CreateDirectory(formFolder);
            }
            return formFolder;
        }
    }
}
