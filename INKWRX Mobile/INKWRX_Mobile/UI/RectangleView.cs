using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormTools.FormDescriptor;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class RectangleView : Frame, IElementView
    {
        public RectangleView(RectangleDescriptor descriptor) : base()
        {
            this.Descriptor = descriptor;
            this.BackgroundColor = descriptor.FillColour.ToColor();
            this.HasShadow = false;
            
        }



        public RectangleDescriptor Descriptor { get; private set; }

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
