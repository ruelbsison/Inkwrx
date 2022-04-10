using System;
using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(InkwrxDatePicker), typeof(InkwrxDatePickerRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
	public class InkwrxDatePickerRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			if (this.Control != null && e.NewElement != null)
			{
				this.Control.Placeholder = ((InkwrxDatePicker)e.NewElement).Format.ToUpper();
				this.setEmptyIfNull();
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == InkwrxDatePicker.NullableDateProperty.PropertyName)
			{
				this.setEmptyIfNull();
			}
		}

        

		private void setEmptyIfNull()
		{
			var picker = (InkwrxDatePicker)this.Element;
			if (picker != null && picker.NullableDate == null)
			{
				this.Control.Text = "";
			}
		}
	}
}
