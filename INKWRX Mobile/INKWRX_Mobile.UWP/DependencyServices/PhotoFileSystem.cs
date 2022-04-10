using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INKWRXPhotoTools_Mobile;
using static INKWRXPhotoTools_Mobile.PhotoTools;
using INKWRX_Mobile.UWP.DependencyServices;
using Windows.Storage.Search;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;

[assembly:Xamarin.Forms.Dependency(typeof(PhotoFileSystem))]
namespace INKWRX_Mobile.UWP.DependencyServices
{
    public class PhotoFileSystem : IPhotoFileSystem
    {
        public async void ClearNoTransactionFolder()
        {
            var transFolder = await this.GetTransactionFolder("-1");
            var files = await transFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }
        }

        public async Task<List<ImageEntry>> GetCameraImages(string transactionId)
        {
            var transFolder = await this.GetTransactionFolder(transactionId);
            var images = (await transFolder.GetFilesAsync()).ToList();
            return images.Select(x => new ImageEntry { CreatedDate = x.DateCreated.DateTime, ImageReference = x.Path, ImageType = ImageEntry.ImageEntryType.Camera }).ToList();
        }

        public async Task<List<ImageEntry>> GetGalleryImages()
        {
            var results = await KnownFolders.PicturesLibrary.GetFilesAsync(CommonFileQuery.OrderByDate);
            var list = results.Select(x => new ImageEntry { CreatedDate = x.DateCreated.DateTime, ImageReference = x.Path, ImageType = ImageEntry.ImageEntryType.Gallery }).ToList();
            return list;
        }

        public async Task<byte[]> GetImage(ImageEntry entry)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(entry.ImageReference);
            byte[] fileBytes = null;

            using (var sourceStream = await storageFile.OpenReadAsync())
            {
                fileBytes = new byte[sourceStream.Size];
                using (var reader = new DataReader(sourceStream))
                {
                    await reader.LoadAsync((uint)sourceStream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        public async Task<byte[]> GetImage(ImageEntry entry, int maxX, int maxY)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(entry.ImageReference);
            
            byte[] fileBytes = null;

            using (var thumb = await storageFile.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView,
                (uint)Math.Min(maxX, maxY)))
            {
                
                var myBuffer = new Windows.Storage.Streams.Buffer(Convert.ToUInt32(thumb.Size));

                var ibuff = await thumb.ReadAsync(myBuffer, myBuffer.Capacity, InputStreamOptions.None);
                
                fileBytes = new byte[thumb.Size];
                
                using (var reader = DataReader.FromBuffer(ibuff))
                {

                    reader.ReadBytes(fileBytes);
                    
                }
            }

            return fileBytes;
        }

        public async Task<ImageEntry> MoveCameraImage(ImageEntry entry, string transactionId)
        {
            var newFolder = await this.GetTransactionFolder(transactionId);
            var storageFile = await StorageFile.GetFileFromPathAsync(entry.ImageReference);
            await storageFile.MoveAsync(newFolder);
            entry.ImageReference = storageFile.Path;
            return entry;
        }

        public async Task<ImageEntry> SaveImage(byte[] image, ImageEntry entry, string transactionId)
        {
            var fileName = string.Format("{0}.png", Guid.NewGuid().ToString());
            StorageFolder folder;
            if (entry.ImageType == ImageEntry.ImageEntryType.Camera)
            {
                folder = await this.GetTransactionFolder(transactionId);
            }
            else
            {
                folder = KnownFolders.DocumentsLibrary;
            }

            var storageFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var readStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (var outputStream = readStream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new DataWriter(outputStream))
                    {
                        dataWriter.WriteBytes(image);
                        await dataWriter.StoreAsync();
                        await dataWriter.FlushAsync();
                    }
                }
            }
            return entry;


        }

        private async Task<StorageFolder> GetTransactionFolder(string transId)
        {
            var appFolder = ApplicationData.Current.LocalFolder;
            var dataFolder = await appFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            var transactionFolder = await dataFolder.CreateFolderAsync("Transactions", CreationCollisionOption.OpenIfExists);
            return await transactionFolder.CreateFolderAsync(transId == "-1" ? "NoTransaction" : transId, CreationCollisionOption.OpenIfExists);

        }


    }
}
