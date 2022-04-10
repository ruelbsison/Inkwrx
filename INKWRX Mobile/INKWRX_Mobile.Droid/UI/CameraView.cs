using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Hardware;
using INKWRX_Mobile.Views;
using Android.Views;
using Android.Graphics;
using Java.Lang;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Hardware.Camera2.Params;
using Java.Util;
using Java.Nio;

namespace INKWRX_Mobile.Droid.UI
{
    public class CameraView : TextureView
    {
        private int RatioWidth = 0;
        private int RatioHeight = 0;

        private CameraDevice CameraDevice;
        private CaptureRequest.Builder PreviewRequestBuilder;

        private bool FlashSupported = false;

        public CameraCaptureListener CaptureListener;
        private CameraCaptureSession CaptureSession;
        private DeviceCallback DeviceCallBack;

        private ImageReader ImageReader;

        private HandlerThread BackgroundThread;
        private Handler BackgroundHandler;

        private Size PreviewSize;
        private string CameraId;

        private CameraPage CameraPage;

        private States State = States.STATE_PREVIEW;

        private OrientationListener orientationListener = null;
        private int Orientation = 0;//orientation of the device
        private int sensorOrientation = 0;//orientation of the cammera on the device

        private bool notSupported = false;
        private bool autofocusNotSupported = false;
        private bool isLegacyLocked = false;

        private enum States
        {
            STATE_PREVIEW,// Camera state: Showing camera preview.
            STATE_WAITING_LOCK,// Camera state: Waiting for the focus to be locked.
            STATE_WAITING_PRECAPTURE,// Camera state: Waiting for the exposure to be precapture state.
            STATE_WAITING_NON_PRECAPTURE,//Camera state: Waiting for the exposure state to be something other than precapture.
            STATE_PICTURE_TAKEN// Camera state: Picture was taken.
        }

        public CameraView(Context context) :
            base(context)
        {
            if (!this.Context.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCamera))
            {
                notSupported = true;
                //device not supported for camera
                new AlertDialog.Builder(this.Context)
                    .SetTitle("Unsupported Camera")
                    .SetMessage("The camera on your phone is not supported by this application. To attach photos, please take photos with the device camera app and attach from gallery.")
                    .SetNeutralButton("Ok", new EventHandler<DialogClickEventArgs>((sender, args) =>
                    {

                    })).Show();
                return;
            }

            this.SurfaceTextureListener = new CustomSurfaceTextureListener(this);

            this.BackgroundThread = new HandlerThread("CameraBackground");
            this.BackgroundThread.Start();
            this.BackgroundHandler = new Handler(BackgroundThread.Looper);

            this.DeviceCallBack = new DeviceCallback(this);
            this.CaptureListener = new CameraCaptureListener(this);

            this.orientationListener = new OrientationListener(context, this);
            if (this.orientationListener.CanDetectOrientation())
            {
                this.orientationListener.Enable();
            }
        }

        public void SetAspectRation(int width, int height)
        {
            this.RatioWidth = width;
            this.RatioHeight = height;
            this.RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);

            if (this.RatioWidth == 0 || this.RatioHeight == 0)
            {
                SetMeasuredDimension(width, height);
            }
            else if (width < (float)height * this.RatioWidth / (float)this.RatioHeight)
            {
                SetMeasuredDimension(width, width * this.RatioHeight / this.RatioWidth);
            }
            else
            {
                SetMeasuredDimension(height * this.RatioWidth / this.RatioHeight, height);
            }
        }

        #region listeners
        private class CustomSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
        {
            private CameraView CameraView;

            public CustomSurfaceTextureListener(CameraView cameraView)
            {
                this.CameraView = cameraView;
            }

            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                this.CameraView.OpenCamera(width, height);
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                //throw new NotImplementedException();
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
                this.CameraView.ConfigureTransform(width, height);
            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
                //throw new NotImplementedException();
            }
        }

        public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
        {
            public CameraView CameraView;

            public CameraCaptureListener(CameraView cameraView)
            {
                this.CameraView = cameraView;
            }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                Process(result);
            }

            public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
            {
                Process(partialResult);
            }

            private void Process(CaptureResult result)
            {
                switch (this.CameraView.State)
                {
                    case CameraView.States.STATE_WAITING_LOCK:
                        {
                            Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                            if (afState == null || this.CameraView.autofocusNotSupported)
                            {
                                this.CameraView.CaptureStillPicture();
                            }

                            else if ((((int)ControlAFState.FocusedLocked) == afState.IntValue()) ||
                                       (((int)ControlAFState.NotFocusedLocked) == afState.IntValue()))
                            {
                                // ControlAeState can be null on some devices
                                Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                                if (aeState == null ||
                                        aeState.IntValue() == ((int)ControlAEState.Converged))
                                {
                                    this.CameraView.CaptureStillPicture();
                                }
                                else
                                {
                                    this.CameraView.RunPrecaptureSequence();
                                }
                            }
                            break;
                        }
                    case CameraView.States.STATE_WAITING_PRECAPTURE:
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                    aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                                    aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                            {
                                this.CameraView.State = CameraView.States.STATE_WAITING_NON_PRECAPTURE;
                            }
                            break;
                        }
                    case CameraView.States.STATE_WAITING_NON_PRECAPTURE:
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                            {
                                this.CameraView.CaptureStillPicture();
                            }
                            break;
                        }
                }
            }
        }

        private class DeviceCallback : CameraDevice.StateCallback
        {
            private CameraView CameraView;

            public DeviceCallback(CameraView cameraView)
            {
                this.CameraView = cameraView;
            }

            public override void OnDisconnected(CameraDevice camera)
            {
                camera.Close();
                this.CameraView.CameraDevice = null;
            }

            public override void OnError(CameraDevice camera, [GeneratedEnum] Android.Hardware.Camera2.CameraError error)
            {
                camera.Close();
                this.CameraView.CameraDevice = null;
            }

            public override void OnOpened(CameraDevice camera)
            {
                this.CameraView.CameraDevice = camera;

                //CreateCameraPreviewSession
                try
                {
                    SurfaceTexture texture = this.CameraView.SurfaceTexture;

                    texture.SetDefaultBufferSize(this.CameraView.PreviewSize.Width, this.CameraView.PreviewSize.Height);

                    Surface surface = new Surface(texture);

                    this.CameraView.PreviewRequestBuilder = this.CameraView.CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                    this.CameraView.PreviewRequestBuilder.AddTarget(surface);

                    List<Surface> surfaceList = new List<Surface>();
                    surfaceList.Add(surface);
                    surfaceList.Add(this.CameraView.ImageReader.Surface);
                    this.CameraView.CameraDevice.CreateCaptureSession(surfaceList, new CaptureSessionCallback(this.CameraView), null);
                }
                catch (CameraAccessException cae)
                {
                    System.Diagnostics.Debug.WriteLine("CameraView - CreateCameraPreviewSession - message: " + cae.Message + ", StackTrace: " + cae.StackTrace);
                }
            }
        }

        private class CaptureSessionCallback : CameraCaptureSession.StateCallback
        {
            private CameraView CameraView;

            public CaptureSessionCallback(CameraView CameraView)
            {
                this.CameraView = CameraView;
            }

            public override void OnConfigured(CameraCaptureSession session)
            {
                this.CameraView.CaptureSession = session;

                this.CameraView.DisplayCameraPreview();
            }

            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                //throw new NotImplementedException();
            }
        }

        private class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            private CameraView CameraView;

            public ImageAvailableListener (CameraView cameraView)
            {
                this.CameraView = cameraView;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                if(this.CameraView.CameraPage.TabletImageView == null)
                {
                    this.CameraView.PostDelayed(() =>
                    {
                        try
                        {
                            //unlock focus
                            if (!this.CameraView.autofocusNotSupported)
                            {
                                this.CameraView.PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                                this.CameraView.PreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Cancel);
                            }
                            this.CameraView.CaptureSession.SetRepeatingRequest(this.CameraView.PreviewRequestBuilder.Build(),
                                    this.CameraView.CaptureListener, this.CameraView.BackgroundHandler);
                        }
                        catch (CameraAccessException cae)
                        {
                            System.Diagnostics.Debug.WriteLine("CameraView - ImageAvailableListener - OnImageAvailable Exception: " + cae.Message + "\n" + cae.StackTrace);
                        }

                        this.CameraView.CameraPage.TakingPicture = false;
                    }, 100);
                }

                Image image = null;
                try
                {
                    image = reader.AcquireNextImage();
                    using (ByteBuffer buffer = image.GetPlanes()[0].Buffer)
                    {
                        byte[] byteArray = new byte[buffer.Capacity()];
                        buffer.Get(byteArray);

                        int orienation = this.CameraView.Orientation + this.CameraView.sensorOrientation;
                        if (orienation == 360)
                        {
                            orienation = 0;
                        }
                        else if (orienation == -90)
                        {
                            orienation = 270;
                        }
                        if (orienation != 0)
                        {
                            //rotate image
                            BitmapFactory.Options options = new BitmapFactory.Options();

                            options.InJustDecodeBounds = false;
                            Bitmap bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length, options);

                            Matrix matrix = new Matrix();
                            matrix.PostRotate(orienation);
                            //rotate image to correct rotation in matrix
                            bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, options.OutWidth, options.OutHeight, matrix, true);//rotate bitmap

                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                            {
                                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                                byteArray = ms.GetBuffer();
                            }
                        }

                        this.CameraView.CameraPage.OnPictureTaken(byteArray);
                    }
                }
                finally
                {
                    if (image != null)
                    {
                        image.Close();//release image from ImageReader to prevent IllegalStateException caused by exceeding max images set in ImageReader.NewInstance
                    }
                }
            }
        }

        private class OrientationListener : OrientationEventListener
        {
            private CameraView CameraView;

            public OrientationListener(Context context, CameraView cameraView) : base(context, SensorDelay.Normal)
            {
                this.CameraView = cameraView;
            }

            public override void OnOrientationChanged(int orientation)
            {
                // determine our orientation based on sensor response

                if (orientation < 45 || orientation >= 315)
                {
                    orientation = 0;
                }
                else if (orientation < 135 && orientation >= 45)
                {
                    orientation = 90;
                }
                else if (orientation < 225 && orientation >= 135)
                {
                    orientation = 180;
                }
                else //if (orientation < 315 && orientation >= 225)
                {
                    orientation = 270;
                }

                if (orientation != this.CameraView.Orientation)
                {
                    this.CameraView.Orientation = orientation;
                }
            }
        }
        #endregion

        #region setup camera
        public class CompareSizesByArea : Java.Lang.Object, IComparator
        {
            public int Compare(Java.Lang.Object lhs, Java.Lang.Object rhs)
            {
                var lhsSize = (Size)lhs;
                var rhsSize = (Size)rhs;
                // We cast here to ensure the multiplications won't overflow
                return Long.Signum((long)lhsSize.Width * lhsSize.Height - (long)rhsSize.Width * rhsSize.Height);
            }
        }

        private void SetAutoFlash()
        {
            if (this.FlashSupported)
            {
                this.PreviewRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
            }
        }

        private void OpenCamera(int width, int height)
        {
            CameraManager manager = (CameraManager)Application.Context.GetSystemService(Context.CameraService);
            SetUpCameraOutputs(width, height, manager);

            try
            {
                manager.OpenCamera(this.CameraId, this.DeviceCallBack, this.BackgroundHandler);
            }
            catch (InterruptedException ie)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - OpenCamera - message: " + ie.Message + ", StackTrace: " + ie.StackTrace);
            }
        }

        private void SetUpCameraOutputs(int width, int height, CameraManager manager)
        {
            try
            {
                string[] cameraIdList = manager.GetCameraIdList();
                foreach (string cameraId in cameraIdList)
                {
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);
                    var facing = (Integer) characteristics.Get(CameraCharacteristics.LensFacing);
                    if (null != facing && facing == Integer.ValueOf((int)LensFacing.Front))
                    {
                        continue;
                    }

                    var minimumFocusDistance = characteristics.Get(CameraCharacteristics.LensInfoMinimumFocusDistance);
                    autofocusNotSupported = minimumFocusDistance == null || ((float)minimumFocusDistance) == 0;

                    isLegacyLocked = ((int) characteristics.Get(CameraCharacteristics.InfoSupportedHardwareLevel)) == (int)InfoSupportedHardwareLevel.Legacy;

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (null == map)
                    {
                        continue;
                    }

                    var imageReaderSize = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)), new CompareSizesByArea());
                    this.ImageReader = ImageReader.NewInstance(imageReaderSize.Width, imageReaderSize.Height, ImageFormatType.Jpeg, 5);
                    this.ImageReader.SetOnImageAvailableListener(new ImageAvailableListener(this), this.BackgroundHandler);
                    
                    SurfaceOrientation displayRotation = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.Rotation;
                    this.sensorOrientation = (int) characteristics.Get(CameraCharacteristics.SensorOrientation);
                    if (((displayRotation == SurfaceOrientation.Rotation0 || displayRotation == SurfaceOrientation.Rotation180) && (this.sensorOrientation == 90 || this.sensorOrientation == 270))
                        || ((displayRotation == SurfaceOrientation.Rotation90 || displayRotation == SurfaceOrientation.Rotation270) && (this.sensorOrientation == 0 || this.sensorOrientation == 180)))
                    {
                        //swappedDimensions
                        int tmp = width;
                        width = height;
                        height = tmp;
                    }

                    Size larger = null;
                    Size smaller = null;
                    Size[] cameraSizeArray = map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture)));
                    foreach (Size size in cameraSizeArray)
                    {
                        if (size.Width > width && size.Height > height)
                        {
                            if (null == larger)
                            {
                                larger = size;
                            }
                            else if ((size.Width * size.Height) < (larger.Width * larger.Height))
                            {
                                larger = size;
                            }
                        }
                        else
                        {
                            if (null == smaller)
                            {
                                smaller = size;
                            }
                            else if ((size.Width * size.Height) > (smaller.Width * smaller.Height))
                            {
                                smaller = size;
                            }
                        }
                    }

                    if (null != larger)
                    {
                        PreviewSize = larger;
                        this.CameraId = cameraId;
                    }
                    else if (null != smaller)
                    {
                        PreviewSize = smaller;
                        this.CameraId = cameraId;
                    }
                    else
                    {
                        PreviewSize = cameraSizeArray[0];
                        this.CameraId = cameraIdList[0];
                    }

                    if (Android.Content.Res.Orientation.Landscape == Resources.Configuration.Orientation)
                    {
                        this.SetAspectRation(this.PreviewSize.Width, this.PreviewSize.Height);
                    }
                    else
                    {
                        this.SetAspectRation(this.PreviewSize.Height, this.PreviewSize.Width);
                    }
                    
                    return;
                }
            }
            catch (Java.Lang.Exception e){
                System.Diagnostics.Debug.WriteLine("CameraView - SetUpCameraOutputs - message: " + e.Message + ", StackTrace: " + e.StackTrace);
            }
        }

        private void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (this == null || this.PreviewSize == null)
            {
                return;
            }
            SurfaceOrientation displayRotation = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, this.PreviewSize.Height, this.PreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if (SurfaceOrientation.Rotation90 == displayRotation || SurfaceOrientation.Rotation270 == displayRotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Java.Lang.Math.Max((float)viewHeight / this.PreviewSize.Height, (float)viewWidth / this.PreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (((int)displayRotation) - 2), centerX, centerY);
            }
            else if (SurfaceOrientation.Rotation180 == displayRotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            this.SetTransform(matrix);
        }

        private void DisplayCameraPreview()
        {
            try
            {
                if (!autofocusNotSupported)
                {
                    // Auto focus should be continuous for camera preview.
                    this.PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                }
                
                // Flash is automatically enabled when necessary.
                this.SetAutoFlash();

                // Finally, we start displaying the camera preview.
                this.CaptureSession.SetRepeatingRequest(this.PreviewRequestBuilder.Build(),
                        this.CaptureListener, this.BackgroundHandler);
            }
            catch (CameraAccessException cae)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - ImageAvailableListener - OnImageAvailable Exception: " + cae.Message + "\n" + cae.StackTrace);
            }
            catch (NullReferenceException)
            {
                //user exited camera
            }
        }
        #endregion

        #region take picture
        public void TakePicture(CameraPage cameraPage)
        {
            if (notSupported)
            {
                cameraPage.TakingPicture = false;
                cameraPage.TakingThenSavingPicture --;
                return;
            }

            this.CameraPage = cameraPage;

            //LockFocus
            try
            {
                // This is how to tell the camera to lock focus.
                if (!autofocusNotSupported)
                {
                    this.PreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
                }

                // Tell #mCaptureCallback to wait for the lock.
                this.State = States.STATE_WAITING_LOCK;

                if (!isLegacyLocked)
                {
                    RunPrecaptureSequence();
                }
                else
                {
                    this.CaptureSession.Capture(this.PreviewRequestBuilder.Build(), this.CaptureListener, this.BackgroundHandler);
                }
            }
            catch (CameraAccessException cae)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - LockFocus Exception: " + cae.Message + "\n" + cae.StackTrace);
            }
        }

        public void RunPrecaptureSequence()
        {
            try {
                // This is how to tell the camera to trigger.
                this.PreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                this.State = States.STATE_WAITING_PRECAPTURE;
                this.CaptureSession.Capture(this.PreviewRequestBuilder.Build(), this.CaptureListener, this.BackgroundHandler);
            }
            catch (CameraAccessException cae)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - RunPrecaptureSequence Exception: " + cae.Message + "\n" + cae.StackTrace);
            }
        }

        public void CaptureStillPicture()
        {
            this.State = CameraView.States.STATE_PICTURE_TAKEN;
            try
            {
                CaptureRequest.Builder captureBuilder = this.CameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                captureBuilder.AddTarget(this.ImageReader.Surface);

                if (!autofocusNotSupported)
                {
                    captureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);
                }
                captureBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);

                int rotation = (int)Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay.Rotation;
                captureBuilder.Set(CaptureRequest.JpegOrientation, rotation);

                this.CaptureSession.StopRepeating();
                this.CaptureSession.Capture(captureBuilder.Build(), this.CaptureListener, this.BackgroundHandler);
            }
            catch (CameraAccessException cae)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - TakePicture - message: " + cae.Message + ", StackTrace: " + cae.StackTrace);
            }
        }
        #endregion

        protected override void OnDetachedFromWindow()
        {
            //CloseCamera
            try
            {
                if (null != this.CaptureSession)
                {
                    this.CaptureSession.Close();
                    this.CaptureSession = null;
                }
                if (null != this.CameraDevice)
                {
                    this.CameraDevice.Close();
                    this.CameraDevice = null;
                }
                if (null != this.ImageReader)
                {
                    this.ImageReader.Close();
                    this.ImageReader = null;
                }
            }
            catch (InterruptedException ie)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - CloseCamera - message: " + ie.Message + ", StackTrace: " + ie.StackTrace);
            }

            this.BackgroundThread.QuitSafely();
            try
            {
                this.BackgroundThread.Join();
                this.BackgroundThread = null;
                this.BackgroundHandler = null;
            } catch (InterruptedException ie)
            {
                System.Diagnostics.Debug.WriteLine("CameraView - OnDetachedFromWindow - message: " + ie.Message + ", StackTrace: " + ie.StackTrace);
            }

            if (this.orientationListener != null)
            {
                this.orientationListener.Disable();
                this.orientationListener.Dispose();
                this.orientationListener = null;
            }
        }
    }
}