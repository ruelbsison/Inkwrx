using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using INKWRX_Mobile.Droid.CustomRenderers;
using INKWRX_Mobile.UI;
using Xamarin.Forms.Platform.Android;
using INKWRX_Mobile.Droid.UI;
using INKWRX_Mobile.Views;

[assembly: ExportRenderer(typeof(CameraFieldView), typeof(CameraRenderer))]
namespace INKWRX_Mobile.Droid.CustomRenderers
{
    class CameraRenderer : ViewRenderer<CameraFieldView, CameraView>
    {
        private CameraView cameraView = null;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraFieldView> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (Control == null)
            {
                this.cameraView = new CameraView(this.Context);
                SetNativeControl(this.cameraView);
            }

            if (elementChangedEventArgs.OldElement != null)
            {
                elementChangedEventArgs.OldElement.TakePictureHandler -= TakePicture;
            }

            if (elementChangedEventArgs.NewElement != null)
            {
                elementChangedEventArgs.NewElement.TakePictureHandler += TakePicture;
            }
        }

        private void TakePicture(CameraPage cameraPage)
        {
            this.cameraView.TakePicture(cameraPage);
        }
    }
}