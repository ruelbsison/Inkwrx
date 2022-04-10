using FormTools.FormDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    class TickBoxFieldView : Frame, IElementView
    {
        public TickBoxDescriptor Descriptor { get; set; }
        private bool isChecked = false;
        private Label Label;

        public event FieldValueChangedEventHandler FieldValueChanged;

        public bool IsChecked {
            get { return this.isChecked; }
            set
            {
                this.isChecked = value;
                this.Label.Text = this.IsChecked ? "\u2713" : "";
            }
        }

        public TickBoxFieldView(TickBoxDescriptor descriptor)
        {
            this.Descriptor = descriptor;
            this.WidthRequest = 24;
            this.HeightRequest = 24;
            this.HorizontalOptions = LayoutOptions.Start;
            this.HasShadow = false;
            this.Padding = new Thickness(1, 1, 1, 1);
			this.BackgroundColor = Color.Black;
            Frame frame = new Frame();
            frame.WidthRequest = 24;
            frame.HeightRequest = 22;
            frame.HorizontalOptions = LayoutOptions.Start;
            frame.HasShadow = false;
            frame.Padding = new Thickness(1, 0, 0, 3);
            frame.BackgroundColor = Color.White;
            this.Content = frame;

            this.Label = new Label();
			this.Label.WidthRequest = 24 - Device.OnPlatform(0, 2, 0);
			this.Label.HeightRequest = 24 - Device.OnPlatform(0, 5, 0);
			this.Label.FontSize = 24 * Device.OnPlatform(1,0.73d,1);
            this.Label.HorizontalTextAlignment = TextAlignment.Center;
            this.Label.VerticalTextAlignment = TextAlignment.Center;
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
				frame.BackgroundColor = CoreAppTools.MandatoryRed;
            }
			frame.Content = this.Label;
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, args) =>
            {
                this.IsChecked = !this.IsChecked;
            };
            this.GestureRecognizers.Add(tapGestureRecognizer);
            frame.GestureRecognizers.Add(tapGestureRecognizer);
            this.Label.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public ElementDescriptor RawDescriptor { get { return Descriptor; } }

        public string FieldValue
        {
            get
            {
                return this.IsChecked
                    ? this.Descriptor.TickedValue
                    : this.Descriptor.NotTickedValue;
            }
            set
            {

            }
        }

        public string FieldNotShownValue
        {
            get
            {
                return this.Descriptor.NotTickedValue;
            }
        }

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
                return this.IsChecked;
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
                //this.Label.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }
    }
}
