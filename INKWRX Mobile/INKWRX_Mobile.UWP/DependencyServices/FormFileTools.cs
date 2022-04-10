using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.UWP.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

[assembly:Xamarin.Forms.Dependency(typeof(FormFileTools))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class FormFileTools : IFormFileTools
    {
        public async void DeleteFormData(string formId, string username)
        {
            var formFolder = await this.GetFormFolder(formId, username);
            try
            {
                await formFolder.DeleteAsync(StorageDeleteOption.Default);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<string> GetFormData(string formId, string username)
        {
            var formFolder = await this.GetFormFolder(formId, username);
            var formdata = (StorageFile)await formFolder.TryGetItemAsync("formdata.txt");
            if (formdata == null)
            {
                return null;
            }
            return await FileIO.ReadTextAsync(formdata);
        }

        public async Task<byte[]> GetImageData(string imageId, string formId, string username)
        {
            var imageName = string.Format("image{0}.jpg", imageId);
            var formFolder = await this.GetFormFolder(formId, username);
            var image = (StorageFile)await formFolder.TryGetItemAsync(imageName);
            if (image == null)
            {
                return null;
            }
            byte[] bytes;
            using (var stream = await image.OpenAsync(FileAccessMode.Read))
            {
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var datareader = new DataReader(inputStream))
                    {
                        var numBytes = await datareader.LoadAsync((uint)stream.Size);
                        bytes = new byte[numBytes];
                        datareader.ReadBytes(bytes);
                    }
                }
            }
            return bytes;
            
        }

        public async Task<string> GetLexiconData(string lexiconId, string formId, string username)
        {
            var lexiconFileName = string.Format("lexicon{0}.txt", lexiconId);
            var formFolder = await this.GetFormFolder(formId, username);
            var lexiconFile = (StorageFile)await formFolder.TryGetItemAsync(lexiconFileName);
            if (lexiconFile == null)
            {
                return null;
            }
            return await FileIO.ReadTextAsync(lexiconFile);
        }

        public async void SaveAndUnzipFormFiles(string formId, string username, byte[] zipData)
        {

            var userFolder = await this.GetUserFolder(username);
            var formfolder = await this.GetFormFolder(formId, username, true);
            var zipFile = await userFolder.CreateFileAsync(formId + ".zip", CreationCollisionOption.ReplaceExisting);
            await formfolder.DeleteAsync();
            await FileIO.WriteBytesAsync(zipFile, zipData);
            try
            {
                ZipFile.ExtractToDirectory(zipFile.Path, formfolder.Path);
            }
            catch (Exception ex)
            {
                return;
            }
            await zipFile.DeleteAsync(StorageDeleteOption.Default);
        }

        private async Task<StorageFolder> GetFormFolder(string formId, string username, bool replace = false)
        {
            var userFolder = await GetUserFolder(username);
            return await userFolder.CreateFolderAsync(formId, replace ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists);
        }

        private async Task<StorageFolder> GetUserFolder(string username)
        {
            var appFolder = ApplicationData.Current.LocalFolder;
            var dataFolder = await appFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            return await dataFolder.CreateFolderAsync(username, CreationCollisionOption.OpenIfExists);
        }
    }
}
