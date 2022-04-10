using FormTools.FormDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class DateTimeFieldView : ContentView, IElementView
    {
        public event FieldValueChangedEventHandler FieldValueChanged;

        public DateTimeFieldView(DateTimeFieldDescriptor descriptor)
        {
            this.Descriptor = descriptor;
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 5
            };
            var nowLabel = new Label
            {
                TextColor = CoreAppTools.SteelBlue,
                Text = "Now",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var clearLabel = new Label
            {
                TextColor = CoreAppTools.SteelBlue,
                Text = "Clear",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var barLabel = new Label
            {
                Text = "|",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            switch (this.Descriptor.LexiconId)
            {
                case 13:
                case 14:
                case 20:
                case 21:
                case 22:
                case 23:
                    //date
                    this.datePicker = new InkwrxDatePicker
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    this.datePicker.Format = descriptor.FdtFormat.Substring(12).Replace("DD", "dd").Replace("YY", "yy");//remove "Numerical - " from the front, convert formatting character case

                    this.datePicker.NullableDate = null;
                    layout.Children.Add(this.datePicker);
                    var nowDate = new TapGestureRecognizer();
                    nowDate.Tapped += (sender, eventArgs) =>
                    {
                        if (IsPrepop)
                        {
                            return;
                        }
                        Device.BeginInvokeOnMainThread(() =>
						{
							this.datePicker.NullableDate = DateTime.Now.AddDays(1);
							this.datePicker.NullableDate = DateTime.Now;
						});
                    };
                    nowLabel.GestureRecognizers.Add(nowDate);

                    var clearDate = new TapGestureRecognizer();
                    clearDate.Tapped += (sender, eventArgs) =>
                    {
                        if (IsPrepop)
                        {
                            return;
                        }
                        Device.BeginInvokeOnMainThread(() =>
						{
                        	this.datePicker.NullableDate = null;
						});
                    };
                    clearLabel.GestureRecognizers.Add(clearDate);
                    break;
                case 15:
                case 18:
                    //time
                    this.timePicker = new InkwrxTimePicker
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    this.timePicker.Format = descriptor.FdtFormat.Substring(12).Replace("MM", "mm").Replace("SS", "ss");
					this.timePicker.NullableTime = null;
                    layout.Children.Add(this.timePicker);
                    var nowTime = new TapGestureRecognizer();
                    nowTime.Tapped += (sender, eventArgs) =>
                    {
                        if (IsPrepop)
                        {
                            return;
                        }
                        var now = DateTime.Now;
                        var ts = new TimeSpan(now.Hour, now.Minute, now.Second);
						Device.BeginInvokeOnMainThread(() =>
						{
							this.timePicker.NullableTime = ts;
						});
                        
                    };
                    nowLabel.GestureRecognizers.Add(nowTime);

                    var clearTime = new TapGestureRecognizer();
                    clearTime.Tapped += (sender, eventArgs) =>
                    {
                        if (IsPrepop)
                        {
                            return;
                        }
                        Device.BeginInvokeOnMainThread(() =>
						{
							this.timePicker.NullableTime = null;
						});
                        
                    };
                    clearLabel.GestureRecognizers.Add(clearTime);


                    break;
            }
            layout.Children.Add(nowLabel);
            layout.Children.Add(barLabel);
            layout.Children.Add(clearLabel);
            this.Content = layout;
            if (descriptor.Mandatory)
			{
				this.Mandatory = true;
			}
            else
            {
                this.Content.BackgroundColor = Color.White;
            }
        }

        private InkwrxDatePicker datePicker = null;
        private InkwrxTimePicker timePicker = null;

        private bool IsPrepop = false;

        public DateTimeFieldDescriptor Descriptor { get; set; }

        public ElementDescriptor RawDescriptor
        {
            get
            {
                return this.Descriptor;
            }
        }

        public string FieldValue
        {
            get
            {
                var dateFormat = this.Descriptor.FdtFormat.Substring(12).Replace("DD", "dd").Replace("YY", "yy").Replace("SS", "ss");
				if (dateFormat.Contains("HH"))
				{
					dateFormat = dateFormat.Replace("MM", "mm");
				}
				if (this.datePicker != null)
				{
					if (this.datePicker.NullableDate == null)
					{
						return "";
					}
					return this.datePicker.NullableDate.Value.ToString(dateFormat);
				}
				else 
				{
					if (this.timePicker.NullableTime == null)
					{
						return "";
					}
					var time = this.timePicker.NullableTime.Value;
					var hours = time.Hours;
					var minutes = time.Minutes;
					var seconds = time.Seconds;
					var ret = dateFormat.Replace("HH", hours.ToString("00"))
					                    .Replace("mm", minutes.ToString("00"))
					                    .Replace("ss", seconds.ToString("00"));
					return ret;
				}
            }
            set
            {
                
                var dateFormat = this.Descriptor.FdtFormat.Substring(12).Replace("DD", "dd").Replace("YY", "yy").Replace("SS", "ss");
                if (dateFormat.Contains("HH"))
                {
                    dateFormat = dateFormat.Replace("MM", "mm");
                }
				if (this.datePicker != null)
                {

                	DateTime dateTme = ConvertString(value, dateFormat);
                    this.datePicker.NullableDate = dateTme;
                }
                else
                {
					TimeSpan span = ConvertTimeSpan(value, dateFormat);
                    this.timePicker.NullableTime = span;
                }
            }
        }
		private static TimeSpan ConvertTimeSpan(string value, string dateFormat)
		{

            var hours = DateTime.Now.Hour;
			var minutes = DateTime.Now.Minute;
			var seconds = DateTime.Now.Second;
			if (dateFormat.Contains("HH"))
			{
				var hourStr = value.Substring(dateFormat.IndexOf("HH"), 2);
				hours = int.Parse(hourStr);
			}
			if (dateFormat.Contains("mm"))
			{
				var minuteStr = value.Substring(dateFormat.IndexOf("mm"));
				minutes = int.Parse(minuteStr);
			}
			if (dateFormat.Contains("ss"))
			{
				var secondStr = value.Substring(dateFormat.IndexOf("ss"));
				seconds = int.Parse(secondStr);
			}

			return new TimeSpan(hours, minutes, seconds);
		}
        private static DateTime ConvertString(string value, string dateFormat)
        {
            var day = DateTime.Now.Day;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var hours = DateTime.Now.Hour;
            var minutes = DateTime.Now.Minute;
            var seconds = DateTime.Now.Second;
            if (dateFormat.Contains("yyyy"))
            {
                var yearStr = value.Substring(dateFormat.IndexOf("yyyy"), 4);
                year = int.Parse(yearStr);
            }
            else if (dateFormat.Contains("yy"))
            {
                var yearStr = value.Substring(dateFormat.IndexOf("yy"), 2);
                var yearint = int.Parse(yearStr);
                yearStr = (yearint < 70 ? "20" : "19") + yearStr;
                year = int.Parse(yearStr);
            }
            if (dateFormat.Contains("MM"))
            {
                var monthStr = value.Substring(dateFormat.IndexOf("MM"), 2);
                month = int.Parse(monthStr);
            }
            if (dateFormat.Contains("dd"))
            {
                var dayStr = value.Substring(dateFormat.IndexOf("dd"), 2);
                day = int.Parse(dayStr);
            }

            return new DateTime(year, month, day, hours, minutes, seconds);
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
                IsPrepop = true;

                if (this.timePicker != null)
                {
                    this.timePicker.TextColor = CoreAppTools.PrepopBlue;
                    this.timePicker.IsEnabled = false;
                }
                else
                {
                    this.datePicker.TextColor = CoreAppTools.PrepopBlue;
                    this.datePicker.IsEnabled = false;
                }
                //var dateFormat = this.Descriptor.FdtFormat.Substring(12).Replace("DD", "dd").Replace("YY", "yy").Replace("SS", "ss");
                //this.FieldValue = ConvertString(value, dateFormat);
                this.FieldValue = value;
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
					this.Content.BackgroundColor = CoreAppTools.MandatoryRed;
                }
                
            }
        }
    }
}
