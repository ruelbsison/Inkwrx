using CoreGraphics;
using Foundation;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.iOS.DependencyServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;

[assembly:Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace INKWRX_Mobile.iOS.DependencyServices
{
    public class ImageResizer : IImageResizer
    {
        public async Task<byte[]> ResizeImage(byte[] image, double factor)
        {
            var uiimage = ImageFromByteArray(image);

            var size = new CGSize(uiimage.Size.Width / factor, uiimage.Size.Height / factor);
            var hasAlpha = false;
            var scale = 0.0f;
            UIGraphics.BeginImageContextWithOptions(size, !hasAlpha, scale);
            uiimage.Draw(new CGRect(CGPoint.Empty, size));
            var scaledImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return scaledImage.AsJPEG().ToArray();
        }

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
    }
}
