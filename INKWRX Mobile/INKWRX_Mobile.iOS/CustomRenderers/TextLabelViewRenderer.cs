using System;
using Foundation;
using INKWRX_Mobile.iOS.CustomRenderers;
using INKWRX_Mobile.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Collections.Generic;

[assembly:ExportRenderer(typeof(TextLabelView), typeof(TextLabelViewRenderer))]
namespace INKWRX_Mobile.iOS.CustomRenderers
{
	public class TextLabelViewRenderer : LabelRenderer
	{

		private Dictionary<string, string> fonts = new Dictionary<string, string> {
			{@"arial narrow", @"ArialNarrow"},
			{@"arial",@"ArialMT"},
			{@"times new roman",@"TimesNewRomanPSMT"},
			{@"times new roman,times",@"TimesNewRomanPSMT"},
			{@"times new roman, times",@"TimesNewRomanPSMT"},
			{@"tahoma", @"Tahoma"}
		};

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);
			if (this.Control != null && e.NewElement != null)
			{
				var labelView = (TextLabelView)e.NewElement;
				if (labelView.FormattedText.Spans.Count > 0)
				{
					if (labelView.Underline)
					{
						var attText = (NSMutableAttributedString)this.Control.AttributedText.MutableCopy();
						attText.AddAttribute(UIStringAttributeKey.UnderlineStyle, 
						                     NSNumber.FromInt32((int)NSUnderlineStyle.Single),
											 new NSRange(0, attText.Length));
						this.Control.AttributedText = attText;
					}
					var firstSection = labelView.Descriptor.BaseSection;
					var firstSectionFont = firstSection.FontName;
					var newFontName = fonts["arial"];
					if (this.fonts.ContainsKey(firstSectionFont.ToLower()))
					{
						newFontName = fonts[firstSectionFont.ToLower()];
					}
					if (firstSection.Bold)
					{
						newFontName += "-Bold";
						if (newFontName == "ArialMT-Bold")
						{
							newFontName = "Arial-BoldMT";
						}
						if (newFontName == "TimesNewRomanPSMT-Bold")
						{
							newFontName = "TimesNewRomanPS-BoldMT";
						}
					}
					var attribs = (NSMutableAttributedString)this.Control.AttributedText.MutableCopy();
					NSRange range = new NSRange();
					var oldFont = (UIFont)attribs.GetAttribute(UIStringAttributeKey.Font, 0, out range);
					var font = UIFont.FromName(newFontName, (oldFont.PointSize /3f) * 4f);
					if (font != null)
					{
						attribs.RemoveAttribute(UIStringAttributeKey.Font, range);
						attribs.AddAttribute(UIStringAttributeKey.Font, font, range);
					}
                    if (firstSection.Italic)
                    {
						((TextLabelView)e.NewElement).HorizontalOptions = LayoutOptions.FillAndExpand;
                        attribs.AddAttribute(UIStringAttributeKey.Obliqueness, NSNumber.FromDouble(0.4), range);
                    }

					this.Control.AttributedText = attribs;
				}
			}
		}

	}
}
