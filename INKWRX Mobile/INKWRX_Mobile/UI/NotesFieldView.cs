using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormTools.FormDescriptor;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class NotesFieldView : Editor, IElementView
    {
        public NotesFieldView(NotesFieldDescriptor descriptor) : base()
        {
            this.Descriptor = descriptor;
            this.HeightRequest = 25 * descriptor.RectElements.Count;
            this.LimitPerLine = (int)Math.Floor(descriptor.Width / 9d);
            this.CharLimit = this.LimitPerLine * descriptor.RectElements.Count;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.TextChanged += (o, e) =>
            {
                this.FieldValueChanged(this, new EventArgs());
            };
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
            }
            else
            {
                this.BackgroundColor = Color.White;
            }
        }

        public NotesFieldDescriptor Descriptor { get; private set; }

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
                return this.Text ?? "";
            }

            set
            {
                this.Text = value;
            }
        }

        public string FieldValValue
        {
            get
            {
                return null;
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
                if (this.isMandatory)
                {
                    this.BackgroundColor = CoreAppTools.MandatoryRed;
                }
            }
        }

        public string PrepopValue
        {
            set
            {
                this.Text = value;
                this.IsEnabled = false;
                this.TextColor = CoreAppTools.PrepopBlue;
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

        public int LimitPerLine { get; private set; }
        public int CharLimit { get; private set; }

        public event FieldValueChangedEventHandler FieldValueChanged;
    }
}
