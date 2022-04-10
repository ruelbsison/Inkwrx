using FormTools.FormDescriptor;
using FormTools.FormDescriptor.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class TextLabelView : Label, IElementView
    {
        public TextLabelView(TextLabelDescriptor desc) : base()
        {
            
            Descriptor = desc;
            var section = this.Descriptor.BaseSection;
            var labelText = "";
            Span labelSpan = null;
            foreach (var subSection in this.Descriptor.BaseSection.Children)
            {
                if (subSection is LabelText)
                {
                    labelText += subSection;
                    continue;
                }
                else
                {
                    labelText += (((LabelSection)subSection).RawString);
                }
            }
            labelText = labelText.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("apos;", "'");

            labelSpan = RenderSection((LabelSection)this.Descriptor.BaseSection);
            labelSpan.Text = labelText;
            this.HorizontalOptions = LayoutOptions.StartAndExpand;
            this.VerticalOptions = LayoutOptions.CenterAndExpand;
            var fs = new FormattedString();
            fs.Spans.Add(labelSpan);
            this.FormattedText = fs;
        }

        public event FieldValueChangedEventHandler FieldValueChanged;

        public bool Underline { get; set; }

        public void updateBaseSection(LabelSection section)
        {
            foreach (var subSection in section.Children.OfType<LabelSection>().ToList())
            {
                this.updateBaseSection(subSection);
                section.Italic = subSection.Italic;
                section.Underline = subSection.Underline;
                section.Bold = subSection.Bold;
                section.FontName = subSection.FontName;
                section.TextColour = subSection.TextColour;
                section.TextSize = subSection.TextSize;
            }
        }

        private Span RenderSection (LabelSection section)
        {
            var span = new Span();
            this.updateBaseSection(section);
            this.Underline = section.Underline;
            span.ForegroundColor = section.TextColour.ToColor();

            if (Device.OnPlatform(true, false, true)){
                //prevent running on android, android does this in custom renderer
                span.FontFamily = section.FontName;
                span.FontSize = section.TextSize;
            }

            span.Text = section.RawString;

            if (section.Bold)
            {
                if (section.Italic)
                {
                    if (Device.OnPlatform(true, false, true))
                    {
                        //prevent running on android, android does bold in custom renderer
                        span.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;
                    }
                    else
                    {
                        span.FontAttributes = FontAttributes.Italic;
                    }
                }
                else
                {
                    if (Device.OnPlatform(true, false, true))
                    {
                        //prevent running on android, android does bold in custom renderer
                        span.FontAttributes = FontAttributes.Bold;
                    }
                }
            }
            else if (section.Italic)
            {
                span.FontAttributes = FontAttributes.Italic;
            }
            return span;
        }

        public TextLabelDescriptor Descriptor { get; set; }

        public ElementDescriptor RawDescriptor
        {
            get
            {
                return Descriptor;
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
                this.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }
    }
}
