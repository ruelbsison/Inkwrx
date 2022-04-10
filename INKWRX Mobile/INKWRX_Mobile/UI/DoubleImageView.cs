using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class DoubleImageView : Image
    {
        public DoubleImageView(ImageSource onImage, ImageSource offImage) : base()
        {
            this.onImageSource = onImage;
            this.offImageSource = offImage;
            this.Aspect = Aspect.AspectFit;
            this.Source = this.offImageSource;
        }

        public bool IsOn
        {
            get
            {
                return this.on;
            }
            set
            {
                this.on = value;
                this.Source = this.on ? this.onImageSource : this.offImageSource;
            }
        }

        public void Toggle ()
        {
            this.IsOn = !this.IsOn;
        }

        private ImageSource onImageSource = null;
        private ImageSource offImageSource = null;

        private bool on = false;
    }
}
