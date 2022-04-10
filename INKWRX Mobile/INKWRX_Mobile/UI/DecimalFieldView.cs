using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormTools.FormDescriptor;
using Xamarin.Forms;
using System.Globalization;

namespace INKWRX_Mobile.UI
{
    public class DecimalFieldView : Entry, IElementView
    {

        public DecimalFieldView(ISOFieldDescriptor descriptor) : base()
        {
            this.Descriptor = descriptor;
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
            }
            else
            {
                this.BackgroundColor = Color.White;
            }
            this.MaxLength = descriptor.RectElements.Count + 1;
            
            this.Keyboard = Keyboard.Numeric;
            
            this.TextChanged += (sender, eventArgs) =>
            {
                this.FieldValueChanged?.Invoke(this, new EventArgs());
            };

            this.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        public ISOFieldDescriptor Descriptor { get; private set; }

        public string FieldNotShownValue
        {
            get
            {
                return "";
            }
        }

        private bool overFlow = false;
        private string actualValue = "";
        public string FieldValue
        {
            get
            {

                return this.overFlow
                    ? this.actualValue
                    : this.Text ?? "";   
            }

            set
            {
                double valueAsDouble = double.Parse(value, CultureInfo.InvariantCulture);

                if (this.Descriptor.FdtListArray.Contains("|"))
                {
                    string[] sections = this.Descriptor.FdtListArray.Split('|');

                    if (sections.Count() > 1)
                    {
                        int charCountDecimal = int.Parse(sections[1]);
                        this.actualValue = Math.Round(valueAsDouble, charCountDecimal).ToString();

                        int charCountInteger = int.Parse(sections[0]);
                        if (this.actualValue.Length > charCountInteger + 1 + charCountDecimal)//+1 for decimal
                        {
                            this.overFlow = true;
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int charItertor = 0; charItertor < charCountInteger; charItertor++)
                            {
                                stringBuilder.Append('#');
                            }
                            stringBuilder.Append('.');
                            for (int charItertor = 0; charItertor < charCountDecimal; charItertor++)
                            {
                                stringBuilder.Append('#');
                            }
                            this.Text = stringBuilder.ToString();
                        }
                        else
                        {
                            this.overFlow = false;
                            this.Text = this.actualValue;
                        }
                    }
                    else
                    {
                        int valueAsInt = (int)Math.Round(valueAsDouble, 0);
                        this.actualValue = valueAsInt.ToString();

                        int charCount = int.Parse(this.Descriptor.FdtListArray);
                        if (this.actualValue.Length > charCount)
                        {
                            this.overFlow = true;
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int charItertor = 0; charItertor < charCount; charItertor++)
                            {
                                stringBuilder.Append('#');
                            }
                            this.Text = stringBuilder.ToString();
                        }
                        else
                        {
                            this.overFlow = false;
                            this.Text = this.actualValue;
                        }
                    }
                }
                else
                {
                    int valueAsInt = (int)Math.Round(valueAsDouble, 0);
                    this.actualValue = valueAsInt.ToString();

                    int charCount = int.Parse(this.Descriptor.FdtListArray);
                    if (this.actualValue.Length > charCount)
                    {
                        this.overFlow = true;
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int charItertor = 0; charItertor < charCount; charItertor++)
                        {
                            stringBuilder.Append('#');
                        }
                        this.Text = stringBuilder.ToString();
                    }
                    else
                    {
                        this.overFlow = false;
                        this.Text = this.actualValue;
                    }
                }
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

        public int MaxLength { get; private set; }

        public string PrepopValue
        {
            set
            {
                this.IsEnabled = false;
                this.TextColor = CoreAppTools.PrepopBlue;
                this.Text = value;
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

        public string OldText { get; set; }

        public event FieldValueChangedEventHandler FieldValueChanged;
    }
}
