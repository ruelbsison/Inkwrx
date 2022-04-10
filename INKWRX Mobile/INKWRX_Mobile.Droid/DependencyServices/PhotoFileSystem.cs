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
using INKWRX_Mobile.Droid.DependencyServices;
using INKWRXPhotoTools_Mobile;
using static INKWRXPhotoTools_Mobile.PhotoTools;
using System.IO;
using Android.Graphics;
using Java.Util;

[assembly: Xamarin.Forms.Dependency(typeof(PhotoFileSystem))]
namespace INKWRX_Mobile.Droid.DependencyServices
{
    class PhotoFileSystem : IPhotoFileSystem
    {
        private static string ImagesFolder = "images";
        private static string NoTransactionFolder = "NoTransaction";

        public async Task<List<ImageEntry>> GetGalleryImages()
        {
            Android.Database.ICursor cursor = null;
            try {
                cursor = Application.Context.ContentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri,
                    new String[]{
                            Android.Provider.MediaStore.Images.Media.InterfaceConsts.DateTaken,
                            Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data,
                            Android.Provider.MediaStore.Images.Media.InterfaceConsts.Orientation
                    },
                    "", null, "");

                if (cursor.MoveToFirst())
                {
                    int dateColumn = cursor.GetColumnIndex(Android.Provider.MediaStore.Images.Media.InterfaceConsts.DateTaken);
                    int dataColumn = cursor.GetColumnIndex(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    int orientationColumn = cursor.GetColumnIndex(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Orientation);

                    List<ImageEntry> imageEntryList = new List<ImageEntry>();
                    do
                    {
                        ImageEntry imageEntry = new ImageEntry();
                        imageEntry.CreatedDate = new DateTime(1970, 1, 1).AddMilliseconds(cursor.GetDouble(dateColumn));
                        imageEntry.ImageReference = cursor.GetString(dataColumn);
                        imageEntry.Orientation = cursor.GetInt(orientationColumn);//file metadata stores orientation
                        imageEntry.ImageType = ImageEntry.ImageEntryType.Gallery;
                        imageEntryList.Add(imageEntry);
                    } while (cursor.MoveToNext());
                    return imageEntryList;
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }
            return new List<ImageEntry>();
        }

        public async Task<List<ImageEntry>> GetCameraImages(string transactionId)
        {
            string imagesDirectory = System.IO.Path.Combine(FormFileTools.GetAppPath(), ImagesFolder);
            if (! Directory.Exists(imagesDirectory))
            {
                return new List<ImageEntry>();
            }

            string imagesTransactionDirectory = System.IO.Path.Combine(imagesDirectory, transactionId);
            if (! Directory.Exists(imagesTransactionDirectory))
            {
                return new List<ImageEntry>();
            }

            IEnumerable<string> directories = Directory.EnumerateDirectories(imagesTransactionDirectory + "/");
            List<ImageEntry> imageEntryList = new List<ImageEntry>();

            foreach (string image in directories)
            {
                ImageEntry imageEntry = new ImageEntry();
                imageEntry.ImageReference = image;
                imageEntry.ImageType = ImageEntry.ImageEntryType.Camera;
                imageEntry.CreatedDate = File.GetCreationTime(image);
                imageEntry.Orientation = 0;
                imageEntryList.Add(imageEntry);
            }

            return imageEntryList;
        }

        public async Task<byte[]> GetImage(ImageEntry entry)
        {
            if(! File.Exists(entry.ImageReference))
            {
                return new byte[0];
            }

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = false;
            options.InSampleSize = 2;

            return GetImageData(entry, options);
        }

        //scales image to dimensions within max width and height of passed paramaters
        public async Task<byte[]> GetImage(ImageEntry entry, int maxWidth, int maxHeight)
        {
            if (! File.Exists(entry.ImageReference))
            {
                return new byte[0];
            }

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(entry.ImageReference, options);

            if (options.OutWidth > maxWidth || options.OutHeight > maxHeight)
            {
                int width = options.OutWidth + 0;
                int height = options.OutHeight + 0;
                int inSampleSize = 1;

                int halfWidth = width / 2;
                int halfHeight = height / 2;
                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfWidth / inSampleSize) > maxWidth
                    || (halfHeight / inSampleSize) > maxHeight)
                {
                    inSampleSize *= 2;
                }

                options.InSampleSize = inSampleSize;
            }
            else
            {
                options.InSampleSize = 1;
            }

            options.InJustDecodeBounds = false;

            return GetImageData(entry, options);
        }

        private byte[] GetImageData(ImageEntry imageEntry, BitmapFactory.Options bitmapOptions)
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(imageEntry.ImageReference, bitmapOptions);

            if (imageEntry.ImageType == ImageEntry.ImageEntryType.Gallery && imageEntry.Orientation == -1)
            {
                //retrieve orientation metadata from gallery
                Android.Database.ICursor cursor = null;
                try
                {
                    cursor = Application.Context.ContentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri,
                        new String[]{
                            Android.Provider.MediaStore.Images.Media.InterfaceConsts.Orientation
                        },
                        string.Format("{0} = '{1}'", Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data, imageEntry.ImageReference),
                        null, "");

                    if (cursor.MoveToFirst())
                    {
                        int orientationColumn = cursor.GetColumnIndex(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Orientation);

                        imageEntry.Orientation = cursor.GetInt(orientationColumn);//file metadata stores orientation
                    }
                    imageEntry.Orientation = 0;
                }
                finally
                {
                    if (cursor != null)
                    {
                        cursor.Close();
                    }
                }
            }
            if (imageEntry.Orientation != 0)
            {
                Matrix matrix = new Matrix();
                matrix.SetRotate(imageEntry.Orientation);
                bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                return stream.ToArray();
            }
        }

        public async Task<ImageEntry> SaveImage(byte[] image, ImageEntry entry, string transactionId)
        {
            if (entry.ImageType == ImageEntry.ImageEntryType.Camera)
            {
                string imagesDirectory = System.IO.Path.Combine(FormFileTools.GetAppPath(), ImagesFolder);
                if (! Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                string imagesTransactionDirectory;
                if (transactionId == "-1")
                {
                    imagesTransactionDirectory = System.IO.Path.Combine(imagesDirectory, NoTransactionFolder);
                }
                else
                {
                    imagesTransactionDirectory = System.IO.Path.Combine(imagesDirectory, transactionId);
                }
                if (! Directory.Exists(imagesTransactionDirectory))
                {
                    Directory.CreateDirectory(imagesTransactionDirectory);
                }

                string filename;
                if (null != entry.ImageReference && entry.ImageReference.EndsWith(".jpg"))
                {
                    filename = entry.ImageReference.Substring(entry.ImageReference.LastIndexOf('/') + 1);
                }
                else
                {
                    filename = UUID.RandomUUID().ToString() + ".jpg";
                }

                string filePath = System.IO.Path.Combine(imagesTransactionDirectory, filename);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (FileStream fileStream = File.Create(filePath))
                {
                    fileStream.Write(image, 0, image.Length);
                }

                entry.ImageReference = filePath;
            }
            else
            {
                string url = Android.Provider.MediaStore.Images.Media.InsertImage(Application.Context.ContentResolver, BitmapFactory.DecodeByteArray(image, 0, image.Length), UUID.RandomUUID().ToString(), "INKWRX_Mobile save to gallery");
                Android.Database.ICursor cursor = null;
                try
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(url);
                    cursor = Application.Context.ContentResolver.Query(uri,
                        new String[]{ Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data },
                        null,
                        null,
                        null);

                    if (cursor.MoveToFirst())
                    {
                        entry.ImageReference = cursor.GetString(cursor.GetColumnIndex(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data));
                    }
                }
                finally
                {
                    if (cursor != null)
                    {
                        cursor.Close();
                    }
                }
            }
            return entry;
        }

        public async Task<ImageEntry> MoveCameraImage(ImageEntry entry, string transactionId)
        {
            string imagesDirectory = System.IO.Path.Combine(FormFileTools.GetAppPath(), ImagesFolder);
            if (!Directory.Exists(imagesDirectory))
            {
                return entry;
            }

            string imagesTransactionDirectory = System.IO.Path.Combine(imagesDirectory, transactionId);
            if (!Directory.Exists(imagesTransactionDirectory))
            {
                Directory.CreateDirectory(imagesTransactionDirectory);
            }

            string oldFilePath = entry.ImageReference;

            ImageEntry entryRet = await SaveImage(await GetImage(entry), entry, transactionId);

            File.Delete(oldFilePath);

            return entryRet;
        }

        public void ClearNoTransactionFolder()
        {
            string imagesDirectory = System.IO.Path.Combine(FormFileTools.GetAppPath(), ImagesFolder);
            if (!Directory.Exists(imagesDirectory))
            {
                return;
            }

            string noTransactionPath = System.IO.Path.Combine(imagesDirectory, NoTransactionFolder);
            if (!Directory.Exists(noTransactionPath))
            {
                return;
            }

            Directory.Delete(noTransactionPath, true);
        }
    }
}