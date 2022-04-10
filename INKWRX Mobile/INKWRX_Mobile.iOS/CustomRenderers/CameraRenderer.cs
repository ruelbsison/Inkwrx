using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using INKWRX_Mobile.UI;

using INKWRX_Mobile.Views;
using INKWRX_Mobile.iOS.CustomRenderers;
using Xamarin.Forms.Platform.iOS;
using INKWRX_Mobile.iOS.UI;

[assembly: ExportRenderer(typeof(CameraFieldView), typeof(CameraRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
    class CameraRenderer : ViewRenderer<CameraFieldView, CameraView>
    {
        private CameraView cameraView = null;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraFieldView> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (Control == null)
            {
                this.cameraView = new CameraView();
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