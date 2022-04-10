using System;
using System.Collections.Generic;
using System.Text;
using INKWRX_Mobile.Views;
using UIKit;
using AVFoundation;
using Foundation;
using CoreGraphics;
using INKWRX_Mobile.iOS.Util;
using System.Threading;
using System.Threading.Tasks;
using CoreMotion;

namespace INKWRX_Mobile.iOS.UI
{
    public class CameraView : UIView
    {
        public CameraView (): base()
        {
            this.AddObserver(this, "bounds", 0, default(IntPtr));
            this.startCamera();

			//this.initializeCamera();
			this.startAccelerometer();
        }

		private async void startAccelerometer()
		{
			await Task.Run(() =>
			{
				var manager = new CMMotionManager();
				manager.AccelerometerUpdateInterval = 0.2;
				manager.StartAccelerometerUpdates(new NSOperationQueue(), (data, error) =>
				{
					if (data != null)
					{
						orientation = Math.Abs(data.Acceleration.Y) < Math.Abs(data.Acceleration.X)
										  ? data.Acceleration.X > 0
											  ? UIDeviceOrientation.LandscapeRight
											  : UIDeviceOrientation.LandscapeLeft
										  : data.Acceleration.Y > 0
											  ? UIDeviceOrientation.PortraitUpsideDown
											  : UIDeviceOrientation.Portrait;

					}
				});
			}).ConfigureAwait(false);
		}

		private UIDeviceOrientation orientation = UIDeviceOrientation.Portrait;

        private async void startCamera()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(100);
                return true;
            }).ConfigureAwait(true);
            this.initializeCamera();
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            base.ObserveValue(keyPath, ofObject, change, context);

            this.captureVideoPreviewLayer.Frame = this.Layer.Bounds;
        }

        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            this.RemoveObserver(this, "bounds");
        }

        private async void initializeCamera()
        {
            var authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (authStatus != AVAuthorizationStatus.Authorized)
            {
                var status = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                if (!status)
                {

                }
            }

            var session = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.PresetPhoto
            };

            this.captureVideoPreviewLayer = new AVCaptureVideoPreviewLayer(session)
            {
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,

            };
            this.captureVideoPreviewLayer.Frame = this.Layer.Bounds;
            this.Layer.AddSublayer(this.captureVideoPreviewLayer);

            this.Layer.MasksToBounds = true;

            var devices = AVCaptureDevice.Devices;

            AVCaptureDevice frontCamera = null;
            AVCaptureDevice backCamera = null;

            foreach (var cam in devices)
            {
                if (cam.HasMediaType(AVMediaType.Video))
                {
                    if (cam.Position == AVCaptureDevicePosition.Back)
                    {
                        backCamera = cam;
                    }
                    else
                    {
                        frontCamera = cam;
                    }
                }
            }

            if (backCamera != null)
            {
				
                NSError error = null;
                var input = AVCaptureDeviceInput.FromDevice(backCamera, out error);
                if (error == null)
                {
                    session.AddInput(input);
                }
            }

            this.stillImageOutput = new AVCaptureStillImageOutput
            {
                OutputSettings = new NSDictionary(AVVideo.CodecJPEG, AVVideo.CodecKey)
            };

            session.AddOutput(this.stillImageOutput);

            session.StartRunning();
        }

        internal void TakePicture(CameraPage cameraPage)
        {
            if (this.captureVideoPreviewLayer.Connection.SupportsVideoOrientation)
            {
     //           switch (this.orientation)
     //           {
					//case UIDeviceOrientation.Portrait:
     //                   this.captureVideoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
     //                   break;
     //               case UIDeviceOrientation.LandscapeLeft:
     //                   this.captureVideoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
     //                   break;
     //               case UIDeviceOrientation.LandscapeRight:
     //                   this.captureVideoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
     //                   break;
     //               default:
     //                   this.captureVideoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
     //                   break;
     //           }
            }
            AVCaptureConnection videoConnection = null;
            foreach (var connection in this.stillImageOutput.Connections)
            {
                foreach (var port in connection.InputPorts)
                {
                    if (port.MediaType == AVMediaType.Video)
                    {
                        videoConnection = connection;
                        break;
                    }
                }

                if (videoConnection != null) break;
            }
            
            this.stillImageOutput.CaptureStillImageAsynchronously(videoConnection, async (sampleBuffer, error) =>
            {
                if (sampleBuffer != null)
                {
                    var imageData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
					var uiimage = UIImage.LoadFromData(imageData);
					imageData = null;
					UIImage rotatedImage = null;
					switch (this.orientation)
					{
						case UIDeviceOrientation.LandscapeLeft:
							rotatedImage = this.rotate(uiimage, UIImageOrientation.Left);
							break;
						case UIDeviceOrientation.LandscapeRight:
							rotatedImage = this.rotate(uiimage, UIImageOrientation.Right);
							break;
						case UIDeviceOrientation.PortraitUpsideDown:
							rotatedImage = this.rotate(uiimage, UIImageOrientation.Down);
							break;
						default: // portrait or unknown - if unknown we just don't rotate it
							rotatedImage = uiimage;
							break;
					}

					await cameraPage.OnPictureTaken(rotatedImage.AsJPEG().ToArray());
                    //this.captureVideoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                }
            });

            
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

			//#region Magic Math.... Do NOT touch, or you will be cursed to always plug the USB in the wrong way the first time

			//switch (orient)
			//{
			//	case UIImageOrientation.Up:
			//		// would get an exact copy of the original
			//		return null;

			//	case UIImageOrientation.UpMirrored:
			//		transform = CGAffineTransform.MakeTranslation(rect.Size.Width, 0);
			//		transform = CGAffineTransform.Scale(transform, -1, 1);
			//		break;

			//	case UIImageOrientation.Down:
			//		transform = CGAffineTransform.MakeTranslation(rect.Size.Width, rect.Size.Height);
			//		transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
			//		break;

			//	case UIImageOrientation.DownMirrored:
			//		transform = CGAffineTransform.MakeTranslation(0, rect.Size.Height);
			//		transform = CGAffineTransform.Scale(transform, 1, -1);
			//		break;

			//	case UIImageOrientation.Left:
			//		//bounds = swapWidthAndHeight(bounds);
			//		transform = CGAffineTransform.MakeTranslation(0, rect.Size.Width);
			//		transform = CGAffineTransform.Rotate(transform, 3f * ((float)Math.PI) / 2f);
			//		break;

			//	case UIImageOrientation.LeftMirrored:
			//		bounds = swapWidthAndHeight(bounds);
			//		transform = CGAffineTransform.MakeTranslation(rect.Size.Height, rect.Size.Width);
			//		transform = CGAffineTransform.Scale(transform, -1, 1);
			//		transform = CGAffineTransform.Rotate(transform, 3f * ((float)Math.PI) / 2f);
			//		break;

			//	case UIImageOrientation.Right:
			//		bounds = swapWidthAndHeight(bounds);
			//		transform = CGAffineTransform.MakeTranslation(rect.Size.Height, 0);
			//		transform = CGAffineTransform.Rotate(transform, ((float)Math.PI) / 2f);
			//		break;

			//	case UIImageOrientation.RightMirrored:
			//		bounds = swapWidthAndHeight(bounds);
			//		transform = CGAffineTransform.MakeScale(-1, 1);
			//		transform = CGAffineTransform.Rotate(transform, ((float)Math.PI) / 2f);
			//		break;

			//	default:
			//		// invalid option supplied, assert false
			//		return null;
			//}



			//#endregion

			UIGraphics.BeginImageContext(bounds.Size);
			ctxt = UIGraphics.GetCurrentContext();
			//ctxt.ConcatCTM(transform);
			switch (orient)
			{
				case UIImageOrientation.Left:
				case UIImageOrientation.LeftMirrored:
					ctxt.ScaleCTM(-1, 1);
					ctxt.TranslateCTM(-rect.Size.Width, 0);
					ctxt.RotateCTM((float)Math.PI );
					ctxt.TranslateCTM(-rect.Size.Width, -rect.Size.Height);
					break;
				case UIImageOrientation.Right:
				case UIImageOrientation.RightMirrored:
					ctxt.ScaleCTM(-1, 1);
					ctxt.TranslateCTM(-rect.Size.Width, 0);
					break;
				default:
					ctxt.ScaleCTM(1, -1);
					ctxt.TranslateCTM(0, -rect.Size.Width);
					break;
			}

			//ctxt.ConcatCTM(transform);
			ctxt.DrawImage(rect, img);
			copy = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return copy;
		}

		private static CGRect swapWidthAndHeight(CGRect rect)
		{
			return new CGRect(rect.Location.X, rect.Location.Y, rect.Size.Height, rect.Size.Width);
		}

        private AVCaptureVideoPreviewLayer captureVideoPreviewLayer = null;
        private AVCaptureStillImageOutput stillImageOutput = null;
    }
}
