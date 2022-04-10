using FormTools.FormDescriptor;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    class RadioButtonFieldView : DoubleImageView, IElementView
    {
        public RadioButtonDescriptor Descriptor { get; set; }

        //TODO replace tmp images with radio button images
        public RadioButtonFieldView(RadioButtonDescriptor descriptor) : base (CoreAppTools.GetImageSource("radioChecked.png"), CoreAppTools.GetImageSource("radioUnChecked.png"))
        {
            this.Descriptor = descriptor;
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
            }
            this.WidthRequest = 32;
            this.HeightRequest = 32;
            this.HorizontalOptions = LayoutOptions.Start;

            var changedTap = new TapGestureRecognizer();
            changedTap.Tapped += (sender, eventArgs) =>
            {
                this.FieldValueChanged?.Invoke(this, new EventArgs());
            };
            this.GestureRecognizers.Add(changedTap);
        }

        public string FieldNotShownValue
        {
            get
            {
                return this.Descriptor.NotTickedValue;
            }
        }

        public string FieldValue
        {
            get
            {
                return this.IsOn
                    ? this.Descriptor.TickedValue
                    : this.Descriptor.NotTickedValue;
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

        public string PrepopValue
        {
            set
            {
                // not needed
            }
        }

        public ElementDescriptor RawDescriptor { get { return Descriptor; } }

        public bool Tickable
        {
            get
            {
                return true;
            }
        }

        public bool Ticked
        {
            get
            {
                return this.IsOn;
            }
        }

        public event FieldValueChangedEventHandler FieldValueChanged;

        private bool isMandatory = false;
        public bool Mandatory
        {
            get
            {
                return this.isMandatory;
            }

            set
            {
                this.isMandatory = value;
                this.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }
    }
}
