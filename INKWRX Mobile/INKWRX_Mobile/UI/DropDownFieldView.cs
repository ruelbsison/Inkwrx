using FormTools.FormDescriptor;
using INKWRX_Mobile.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class DropDownFieldView : Picker, IElementView
    {
        private DropdownDescriptor Descriptor;
        private Dictionary<string, string> DropdownDictionary;

        public event FieldValueChangedEventHandler FieldValueChanged;

        public DropDownFieldView(DropdownDescriptor descriptor)
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
            //this.WidthRequest = this.Descriptor.Width;
            //this.HeightRequest = this.Descriptor.Height;
            this.SelectedIndexChanged += DropDownFieldViewChanged;
            this.Title = this.Descriptor.FdtFieldName;
        }

        private void DropDownFieldViewChanged(object sender, EventArgs eventArgs)
        {
            this.FieldValueChanged?.Invoke(this, new EventArgs());
        }

        public async void ProcessLexicon(long formId, string username)
        {
            
            string lexiconData = await DependencyService.Get<IFormFileTools>().GetLexiconData(this.Descriptor.LexiconId, formId.ToString(), username);
            lexiconData = lexiconData.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("apos;", "'");
            this.DropdownDictionary = new Dictionary<string, string>();
            this.DropdownDictionary.Add(" ", "");
            string[] lineArray = lexiconData.Split('\n');
            this.Items.Add(" ");
            foreach (string line in lineArray)
            {
                if (line.Contains("="))
                {
                    string[] sections = line.Split('=');
                    string item = sections[0].Trim();
                    this.DropdownDictionary.Add(item, sections[1].Trim());
                    this.Items.Add(item);
                }
                else
                {
                    string item = line.Trim();
                    this.DropdownDictionary.Add(item, "");
                    this.Items.Add(item);
                }
            }
            this.SelectedIndex = 0;
            
        }

        public ElementDescriptor RawDescriptor { get { return Descriptor; } }

        public string FieldValue
        {
            get
            {
                if (this.SelectedIndex == -1)
                {
                    return "";
                }
                return this.Items[this.SelectedIndex].Trim();
            }
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    this.SelectedIndex = -1;
                    return;
                }
                this.SelectedIndex = this.Items.IndexOf(value);
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
                return this.DropdownDictionary[this.Items[this.SelectedIndex]];
            }
        }

        public string PrepopValue
        {
            set
            {
                this.TextColor = CoreAppTools.PrepopBlue;
                this.IsEnabled = false;
                this.SelectedIndex = this.Items.IndexOf(value);
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
                this.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }
    }
}
