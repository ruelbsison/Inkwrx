using INKWRX_Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class CameraFieldView : View
    {
        public event TakePictureEventHandler TakePictureHandler;

        //public CameraFieldView() : base()
        //{
        //Content = new Label { Text = "Hello View" };
        //}

        public void TakePicture(CameraPage cameraPage)
        {
            TakePictureHandler(cameraPage);
        }

        public delegate void TakePictureEventHandler(CameraPage cameraPage);
    }
}
