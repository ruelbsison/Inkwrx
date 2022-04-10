using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views;
using Xamarin.Forms.Platform.UWP;
using INKWRX_Mobile.UWP.UI;
using INKWRX_Mobile.UWP.CustomRenderers;

[assembly: ExportRenderer(typeof(CameraFieldView), typeof(CameraRenderer))]
namespace INKWRX_Mobile.UWP.CustomRenderers
{
    class CameraRenderer : ViewRenderer<CameraFieldView, CameraView>
    {
        private CameraView cameraView = null;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraFieldView> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (Control == null && elementChangedEventArgs.OldElement == null)
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