using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class InkwrxTimePicker : TimePicker
    {
		private string _format = null;
		public static readonly BindableProperty NullableTimeProperty = BindableProperty.Create<InkwrxTimePicker, TimeSpan?>(p => p.NullableTime, null);

		public TimeSpan? NullableTime
		{
			get
			{
				return (TimeSpan?)GetValue(NullableTimeProperty);
			}
			set
			{
				SetValue(NullableTimeProperty, value);
				UpdateDate();
			}
		}

		private void UpdateDate()
		{
			if (NullableTime.HasValue)
			{
				if (null != _format)
				{
					Format = _format;
					Time = NullableTime.Value;
				}
			}
			else
			{
				_format = Format;
			}
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			UpdateDate();
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

            if (propertyName == "Time")
            {
                if (!NullableTime.HasValue || !NullableTime.Value.Equals(Time))
                {
                    NullableTime = Time;
                }
            }
		}
    }
}
