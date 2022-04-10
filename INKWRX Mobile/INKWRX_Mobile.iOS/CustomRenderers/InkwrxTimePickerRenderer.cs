using System;
using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(InkwrxTimePicker), typeof(InkwrxTimePickerRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
	public class InkwrxTimePickerRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);
			if (this.Control != null && e.NewElement != null)
			{
				this.Control.Placeholder = ((InkwrxTimePicker)e.NewElement).Format.ToUpper();
				this.setEmptyIfNull();
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == InkwrxTimePicker.NullableTimeProperty.PropertyName)
			{
				this.setEmptyIfNull();
			}
		}

		private void setEmptyIfNull()
		{
			var picker = (InkwrxTimePicker)this.Element;
			if (picker != null && picker.NullableTime == null)
			{
				this.Control.Text = "";
			}
		}
	}
}
