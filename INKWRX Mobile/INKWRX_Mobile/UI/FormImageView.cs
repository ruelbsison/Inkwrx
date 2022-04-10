using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormTools.FormDescriptor;
using Xamarin.Forms;
using INKWRX_Mobile.Dependencies;
using System.IO;

namespace INKWRX_Mobile.UI
{
    public class FormImageView : Image, IElementView
    {
        public FormImageView(ImageDescriptor descriptor, string formId, string username)
        {
            this.Descriptor = descriptor;
            this.setImage(formId, username);
            this.Aspect = Aspect.AspectFit;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var sizeDifference = widthConstraint / this.Descriptor.Width;
            return new SizeRequest(new Size(widthConstraint, heightConstraint * sizeDifference));
        }

        private async void setImage(string formId, string username)
        {
            var image = await DependencyService.Get<IFormFileTools>().GetImageData(this.Descriptor.ImageId, formId, username);
            if (image != null)
            {
                this.Source = ImageSource.FromStream(() => new MemoryStream(image));
            }
        }

        public ImageDescriptor Descriptor { get; private set; }

        public string FieldNotShownValue
        {
            get
            {
                return "";
            }
        }

        public string FieldValue
        {
            get
            {
                return "";
            }

            set
            {
                
            }
        }

        public string FieldValValue
        {
            get
            {
                return null;
            }
        }

        public bool Mandatory
        {
            get
            {
                return false;
            }

            set
            {
                
            }
        }

        public string PrepopValue
        {
            set
            {
                
            }
        }

        public ElementDescriptor RawDescriptor
        {
            get
            {
                return this.Descriptor;
            }
        }

        public bool Tickable
        {
            get
            {
                return false;
            }
        }

        public bool Ticked
        {
            get
            {
                return false;
            }
        }

        public event FieldValueChangedEventHandler FieldValueChanged;
    }
}
