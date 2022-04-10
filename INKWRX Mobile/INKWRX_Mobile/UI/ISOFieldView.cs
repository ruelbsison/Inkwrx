using FormTools.FormDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace INKWRX_Mobile.UI
{
    public class ISOFieldView : Entry, IElementView
    {
        public ISOFieldView(ISOFieldDescriptor descriptor) : base()
        {
            Descriptor = descriptor;
            if (descriptor.Mandatory)
            {
                this.Mandatory = true;
            }
            else
            {
                this.BackgroundColor = Color.White;
            }
            this.MaxLength = descriptor.RectElements.Count;
            this.TextChanged += OnTextChanged;
            this.FdtFormat = descriptor.FdtFormat.ToLower();
            this.WidthRequest = descriptor.Width;//has maximum of parent width

            if (this.FdtFormat.Contains("alpha"))
            {
                AllowsText = true;
                if (this.FdtFormat.Contains("num"))
                {
                    AllowsNumber = true;
                    this.Keyboard = Keyboard.Plain;
                }
                else
                {
                    AllowsNumber = false;
                    this.Keyboard = Keyboard.Text;
                }

                if (this.FdtFormat.Contains("uppercase"))
                {
                    if (!this.FdtFormat.Contains("lowercase"))
                    {
                        CapsOnly = true;
                    }
                    else
                    {
                        AllowsLower = true;
                    }
                }
            }
            else
            {
                AllowsNumber = true;
                this.Keyboard = Keyboard.Numeric;
            }

            this.TextChanged += (sender, eventArgs) => 
            {
                this.FieldValueChanged?.Invoke(this, new EventArgs());
            };


            this.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.HorizontalOptions = LayoutOptions.Start;
        }
        public bool Changing = false;
        public string OldText = "";
        

        public int MaxLength { get; private set; }

        public ISOFieldDescriptor Descriptor;

        public string FdtFormat = "";
        public bool CapsOnly = false;
        public bool AllowsText = false;
        public bool AllowsNumber = false;
        public bool AllowsLower = false;

        private bool isMandatory = false;

        public event FieldValueChangedEventHandler FieldValueChanged;

        public bool Mandatory
        {
            get
            {
                return isMandatory;
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

        public ElementDescriptor RawDescriptor
        {
            get
            {
                return Descriptor;
            }
        }

        private string actualValue = "";
        public string FieldValue
        {
            get
            {
                if (this.Descriptor.IsCalcField)
                {
                    return actualValue;
                }
                else {
                    return this.Text ?? "";
                }
            }
            set
            {
                if (this.Descriptor.IsCalcField)
                {
                    double valueAsDouble = double.Parse(value, CultureInfo.InvariantCulture);

                    if (this.Descriptor.FdtListArray.Contains("|"))
                    {
                        string[] sections = this.Descriptor.FdtListArray.Split('|');

                        if (sections.Count() > 1)
                        {
                            int charCountDecimal = int.Parse(sections[1]);
                            this.actualValue = Math.Round(valueAsDouble, charCountDecimal) + "";

                            int charCountInteger = int.Parse(sections[0]);
                            if (this.actualValue.Length > charCountInteger + 1 + charCountDecimal)//+1 for decimal
                            {
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
                                this.Text = this.actualValue;
                            }
                        }
                        else
                        {
                            int valueAsInt = (int)Math.Round(valueAsDouble, 0);
                            this.actualValue = valueAsInt + "";

                            int charCount = int.Parse(this.Descriptor.FdtListArray);
                            if (this.actualValue.Length > charCount)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int charItertor = 0; charItertor < charCount; charItertor++)
                                {
                                    stringBuilder.Append('#');
                                }
                                this.Text = stringBuilder.ToString();
                            }
                            else
                            {
                                this.Text = this.actualValue;
                            }
                        }
                    }
                    else
                    {
                        int valueAsInt = (int) Math.Round(valueAsDouble, 0);
                        this.actualValue = valueAsInt + "";

                        int charCount = int.Parse(this.Descriptor.FdtListArray);
                        if (this.actualValue.Length > charCount)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            for (int charItertor = 0; charItertor < charCount; charItertor++)
                            {
                                stringBuilder.Append('#');
                            }
                            this.Text = stringBuilder.ToString();
                        }
                        else
                        {
                            this.Text = this.actualValue;
                        }
                    }
                }
                else
                {
                    this.Text = value;
                }
            }
        }

        public string FieldNotShownValue
        {
            get
            {
                return "";
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
                this.IsEnabled = false;
                this.TextColor = CoreAppTools.PrepopBlue;
                this.Text = value;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (!this.Descriptor.IsCalcField && this.Text.Length > this.MaxLength)
            {
                this.TextChanged -= OnTextChanged;
                this.Text = eventArgs.OldTextValue;
                this.TextChanged += OnTextChanged;
            }
        }
    }
}
