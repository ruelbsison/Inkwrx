using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using INKWRXPhotoTools_Mobile;
using static INKWRXPhotoTools_Mobile.PhotoTools;
using Photos;
using INKWRX_Mobile.iOS.Util;
using CoreGraphics;
using UIKit;
using System.Threading;
using Foundation;
using INKWRX_Mobile.iOS.DependencyServices;
using System.IO;
using System.Drawing;

[assembly: Xamarin.Forms.Dependency(typeof(PhotoFileSystem))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
	public class PhotoFileSystem : IPhotoFileSystem
	{
        public async Task<List<ImageEntry>> GetGalleryImages()
        {

            if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
            {
                var status = await PHPhotoLibrary.RequestAuthorizationAsync();
                if (status != PHAuthorizationStatus.Authorized)
                {
                    return new List<ImageEntry>();
                }
            }

            var opts = new PHFetchOptions();
            opts.IncludeAssetSourceTypes = PHAssetSourceType.UserLibrary | PHAssetSourceType.CloudShared;
            var results = PHAsset.FetchAssets(PHAssetMediaType.Image, opts);
            var ret = new List<ImageEntry>();
            foreach (PHAsset asset in results)
            {
                var nsdate = asset.CreationDate;
                var cdate = nsdate.AsDateTime();

                ret.Add(new ImageEntry
                {
                    CreatedDate = cdate,
                    ImageReference = asset.LocalIdentifier,
                    ImageType = ImageEntry.ImageEntryType.Gallery
                });
            }
            return ret;

        }

		public async Task<byte[]> GetImage(ImageEntry entry)
		{
			if (entry.ImageType == ImageEntry.ImageEntryType.Gallery)
			{

				#region Gallery Images

				if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
				{
					var status = await PHPhotoLibrary.RequestAuthorizationAsync();
					if (status != PHAuthorizationStatus.Authorized)
					{
						return new byte[0];
					}
				}

				var opts = new PHFetchOptions();
				opts.IncludeAssetSourceTypes = PHAssetSourceType.UserLibrary | PHAssetSourceType.CloudShared;
				var results = PHAsset.FetchAssetsUsingLocalIdentifiers(new string[] { entry.ImageReference }, opts);
				if (results.Count == 0)
				{
					return new byte[] { };
				}
				var item = (PHAsset)results.firstObject;
				var size = new CGSize(item.PixelWidth, item.PixelHeight);
				var imgOpts = new PHImageRequestOptions();
				imgOpts.DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat;
				imgOpts.ResizeMode = PHImageRequestOptionsResizeMode.Exact;
				imgOpts.NetworkAccessAllowed = true;
				imgOpts.Synchronous = true;
				byte[] data = null;

				var imgGot = false;

				PHImageManager.DefaultManager.RequestImageForAsset(item, size, PHImageContentMode.AspectFit, imgOpts, (img, info) =>
				{
					UIImage uiimg = null;
					switch (img.Orientation)
					{
						case UIImageOrientation.Up:
							uiimg = img;
							break;
						default:
							UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);
							img.Draw(new CGRect(0, 0, img.Size.Width, img.Size.Height));
							uiimg = UIGraphics.GetImageFromCurrentImageContext();
							UIGraphics.EndImageContext();
							break;
					}

					var fileData = uiimg.AsPNG();
					data = fileData.ToByteArray();
					imgGot = true;
				});

				while (!imgGot)
				{
					Thread.Sleep(500);
				}

				return data;

				#endregion

			}
			else
			{

				#region Camera Images... No, really.

				if (!File.Exists(entry.ImageReference))
				{
					return new byte[0];
				}
				var bytes = File.ReadAllBytes(entry.ImageReference);
				var img = ImageFromByteArray(bytes);
				UIImage uiimage = null;
				switch (img.Orientation)
				{
					case UIImageOrientation.Up:
						uiimage = img;
						break;
					default:
						UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);
						img.Draw(new CGRect(0, 0, img.Size.Width, img.Size.Height));
						uiimage = UIGraphics.GetImageFromCurrentImageContext();
						UIGraphics.EndImageContext();
						break;
				}
				return uiimage.AsPNG().ToByteArray();

				#endregion
			}
		}

		public async Task<byte[]> GetImage(ImageEntry entry, int maxX, int maxY)
		{
			if (entry.ImageType == ImageEntry.ImageEntryType.Gallery)
			{
				#region Gallery Images
				if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
				{
					var status = await PHPhotoLibrary.RequestAuthorizationAsync();
					if (status != PHAuthorizationStatus.Authorized)
					{
						return new byte[0];
					}
				}
				var opts = new PHFetchOptions();
				opts.IncludeAssetSourceTypes = PHAssetSourceType.UserLibrary | PHAssetSourceType.CloudShared;
				var results = PHAsset.FetchAssetsUsingLocalIdentifiers(new string[] { entry.ImageReference }, opts);
				if (results.Count == 0)
				{
					return new byte[] { };
				}
				var item = (PHAsset)results.firstObject;
				var size = new CGSize(maxX, maxY);
				var imgOpts = new PHImageRequestOptions();
				imgOpts.DeliveryMode = PHImageRequestOptionsDeliveryMode.FastFormat;
				imgOpts.ResizeMode = PHImageRequestOptionsResizeMode.Exact;
				imgOpts.NetworkAccessAllowed = true;
				imgOpts.Synchronous = true;
				byte[] data = null;

				var imgGot = false;

				PHImageManager.DefaultManager.RequestImageForAsset(item, size, PHImageContentMode.AspectFit, imgOpts, (img, info) =>
				{
					UIImage uiimg = null;
					switch (img.Orientation)
					{
						case UIImageOrientation.Up:
							uiimg = img;
							break;
						default:
							UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);
							img.Draw(new CGRect(0, 0, img.Size.Width, img.Size.Height));
							uiimg = UIGraphics.GetImageFromCurrentImageContext();
							UIGraphics.EndImageContext();
							break;
					}

					var fileData = uiimg.AsPNG();
					data = fileData.ToByteArray();
					imgGot = true;
				});

				while (!imgGot)
				{
					Thread.Sleep(500);
				}

				return data;

				#endregion

			}
			else
			{

				#region Camera Images

				if (!File.Exists(entry.ImageReference))
				{
					return new byte[0];
				}
				var imageBytes = File.ReadAllBytes(entry.ImageReference);
				if (imageBytes.Length == 0)
				{
					return imageBytes;
				}
				return ResizeImage(imageBytes, maxX, maxY);

				#endregion

			}
		}

		public static byte[] ResizeImage(byte[] imageData, float width, float height)
		{
			// Load the bitmap
			UIImage originalImage = ImageFromByteArray(imageData);
			//
			var originalHeight = originalImage.Size.Height;
			var originalWidth = originalImage.Size.Width;
			//
			nfloat newHeight = 0;
			nfloat newWidth = 0;
			//

			if (originalHeight > originalWidth)
			{
				newHeight = height;
				nfloat ratio = originalHeight / height;
				newWidth = originalWidth / ratio;
			}
			else
			{
				newWidth = width;
				nfloat ratio = originalWidth / width;
				newHeight = originalHeight / ratio;
			}
			//
			width = (float)newWidth;
			height = (float)newHeight;
			//
			UIGraphics.BeginImageContext(new SizeF(width, height));
			originalImage.Draw(new RectangleF(0, 0, width, height));
			var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			//
			var imageBytes = resizedImage.AsJPEG().ToArray();
			resizedImage.Dispose();
			return imageBytes;
		}
		//
		public static UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			//
			UIImage image;
			try
			{
				image = new UIImage(NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}

		public async Task<ImageEntry> SaveImage(byte[] image, ImageEntry entry, string transactionId)
		{
			bool imageSaved = false;
			if (entry.ImageType == ImageEntry.ImageEntryType.Gallery)
			{
				if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
				{
					var status = await PHPhotoLibrary.RequestAuthorizationAsync();
					if (status != PHAuthorizationStatus.Authorized)
					{
						return null;
					}
				}

				var uiimage = new UIImage(NSData.FromArray(image));
				//UIImage rotatedImage = null;
				//switch (UIApplication.SharedApplication.StatusBarOrientation)
				//{
				//	case UIInterfaceOrientation.LandscapeLeft:
				//		rotatedImage = this.rotate(uiimage, UIImageOrientation.Left);
				//		break;
				//	case UIInterfaceOrientation.LandscapeRight:
				//		rotatedImage = this.rotate(uiimage, UIImageOrientation.Right);
				//		break;
				//	case UIInterfaceOrientation.PortraitUpsideDown:
				//		rotatedImage = this.rotate(uiimage, UIImageOrientation.Down);
				//		break;
				//	default: // portrait or unknown - if unknown we just don't rotate it
				//		rotatedImage = uiimage;
				//		break;
				//}

				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
				{
					// This is created as a var seperately, intentionally - creating it begins the iOS system saving the image
					var req = PHAssetChangeRequest.FromImage(uiimage);
					entry.ImageReference = req.PlaceholderForCreatedAsset.LocalIdentifier;

				},
				(success, error) =>
				{
					if (success)
					{
						entry.CreatedDate = DateTime.Now;
					}
					imageSaved = true;
				});
			}
			else
			{
				var transactionFolder = this.GetTransactionFolder(transactionId);
				var fileName = Guid.NewGuid() + ".jpg";
				var filePath = Path.Combine(transactionFolder, fileName);
				while (File.Exists(filePath))
				{
					fileName = Guid.NewGuid() + ".jpg";
					filePath = Path.Combine(transactionFolder, fileName);
				}
				File.Create(filePath).Close();
				File.WriteAllBytes(filePath, image);
				entry.ImageReference = filePath;
				imageSaved = true;
			}

			while (!imageSaved)
			{
				Thread.Sleep(500);
			}

			return entry;
		}

		private UIImage rotate(UIImage image, UIImageOrientation orient)
		{
			var bounds = CGRect.Empty;
			UIImage copy = null;
			CGContext ctxt = null;
			var img = image.CGImage;
			var rect = CGRect.Empty;
			var transform = CGAffineTransform.MakeIdentity();

			rect.Size = new CGSize(img.Width, img.Height);
			bounds = rect;

			#region Magic Math.... Do NOT touch, or you will be cursed to always plug the USB in the wrong way the first time

			switch (orient)
			{
				case UIImageOrientation.Up:
					// would get an exact copy of the original
					return null;

				case UIImageOrientation.UpMirrored:
					transform = CGAffineTransform.MakeTranslation(rect.Size.Width, 0);
					transform = CGAffineTransform.Scale(transform, -1, 1);
					break;

				case UIImageOrientation.Down:
					transform = CGAffineTransform.MakeTranslation(rect.Size.Width, rect.Size.Height);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
					break;

				case UIImageOrientation.DownMirrored:
					transform = CGAffineTransform.MakeTranslation(0, rect.Size.Height);
					transform = CGAffineTransform.Scale(transform, 1, -1);
					break;

				case UIImageOrientation.Left:
					bounds = swapWidthAndHeight(bounds);
					transform = CGAffineTransform.MakeTranslation(0, rect.Size.Width);
					transform = CGAffineTransform.Rotate(transform, 3f * ((float)Math.PI) / 2f);
					break;

				case UIImageOrientation.LeftMirrored:
					bounds = swapWidthAndHeight(bounds);
					transform = CGAffineTransform.MakeTranslation(rect.Size.Height, rect.Size.Width);
					transform = CGAffineTransform.Scale(transform, -1, 1);
					transform = CGAffineTransform.Rotate(transform, 3f * ((float)Math.PI) / 2f);
					break;

				case UIImageOrientation.Right:
					bounds = swapWidthAndHeight(bounds);
					transform = CGAffineTransform.MakeTranslation(rect.Size.Height, 0);
					transform = CGAffineTransform.Rotate(transform, ((float)Math.PI) / 2f);
					break;

				case UIImageOrientation.RightMirrored:
					bounds = swapWidthAndHeight(bounds);
					transform = CGAffineTransform.MakeScale(-1, 1);
					transform = CGAffineTransform.Rotate(transform, ((float)Math.PI) / 2f);
					break;

				default:
					// invalid option supplied, assert false
					return null;
			}

			#endregion

			UIGraphics.BeginImageContext(bounds.Size);
			ctxt = UIGraphics.GetCurrentContext();

			switch (orient)
			{
				case UIImageOrientation.Left:
				case UIImageOrientation.LeftMirrored:
				case UIImageOrientation.Right:
				case UIImageOrientation.RightMirrored:
					ctxt.ScaleCTM(-1, 1);
					ctxt.TranslateCTM(-rect.Size.Height, 0);
					break;
				default:
					ctxt.ScaleCTM(1, -1);
					ctxt.TranslateCTM(0, -rect.Size.Height);
					break;
			}

			ctxt.ConcatCTM(transform);
			ctxt.DrawImage(rect, img);
			copy = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return copy;
		}

		private static CGRect swapWidthAndHeight(CGRect rect)
		{
			return new CGRect(rect.Location.X, rect.Location.Y, rect.Size.Height, rect.Size.Width);
		}

		public async Task<List<ImageEntry>> GetCameraImages(string transactionId)
		{
			var folder = this.GetTransactionFolder(transactionId);
			var files = new List<string>(Directory.GetFiles(folder));
			if (transactionId != "-1")
			{
				// get the no transaction files too
				var noTransFolder = this.GetTransactionFolder("-1");
				var noTransFiles = Directory.GetFiles(noTransFolder);
				if (noTransFiles.Length > 0)
				{
					files.AddRange(noTransFiles);
				}
			}
			var list = new List<ImageEntry>();
			foreach (var file in files)
			{
				list.Add(new ImageEntry
				{
					ImageType = ImageEntry.ImageEntryType.Camera,
					ImageReference = file,
					CreatedDate = File.GetCreationTime(file)
				});
			}

			return list;
		}

		public string GetTransactionFolder(string transactionId)
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var library = Path.Combine(documents, "..", "Library");
			var dataFolder = Path.Combine(library, "Data");
			if (!Directory.Exists(dataFolder))
			{
				Directory.CreateDirectory(dataFolder);
			}
			var transactionsFolder = Path.Combine(dataFolder, "Transactions");
			if (!Directory.Exists(transactionsFolder))
			{
				Directory.CreateDirectory(transactionsFolder);
			}
			var transFolder = transactionId == "-1" ? "NoTransaction" : transactionId;
			var transFolderPath = Path.Combine(transactionsFolder, transFolder);
			if (!Directory.Exists(transFolderPath))
			{
				Directory.CreateDirectory(transFolderPath);
			}
			return transFolderPath;
		}

        public async Task<ImageEntry> MoveCameraImage(ImageEntry entry, string transactionId)
        {
            var transFolder = this.GetTransactionFolder(transactionId);
            var fileName = Path.GetFileName(entry.ImageReference);
            var newFileName = Path.Combine(transFolder, fileName);
            File.Move(entry.ImageReference, newFileName);
            entry.ImageReference = newFileName;
            return entry;
        }

        public void ClearNoTransactionFolder()
        {
            var transFolder = this.GetTransactionFolder("-1");
            var files = Directory.GetFiles(transFolder);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}