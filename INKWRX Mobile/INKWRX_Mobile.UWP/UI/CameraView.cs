using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INKWRX_Mobile.Views;
using Windows.UI.Xaml.Controls;

namespace INKWRX_Mobile.UWP.UI
{
    public class CameraView : UserControl
    {
        public CameraView() : base()
        {
            this.Content = new StackPanel();
        }

        internal void TakePicture(CameraPage cameraPage)
        {
            cameraPage.OnPictureTaken(null);
        }
    }
}
