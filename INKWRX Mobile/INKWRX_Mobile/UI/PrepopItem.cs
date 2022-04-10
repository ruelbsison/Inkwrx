using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class PrepopItem : ViewCell
    {
        public PrepopItem ()
        {
            this.PrepopFormLabel = new Label
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Start,
                FontSize = 12
            };
            this.PrepopNameLabel = new Label
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Start,
                FontSize = 12
            };

            PrepopFormLabel.SetBinding(Label.TextProperty, "FormName");
            PrepopNameLabel.SetBinding(Label.TextProperty, "PrepopName");

            this.View = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Padding = 5,
                Children =
                {
                    new Image
                    {
                        Source = CoreAppTools.GetImageSource("Icons/HomeScreen/iw_app_ios_icon_prepop.png"),
                        HeightRequest = 70,
                        WidthRequest = 70,
                        Aspect = Aspect.AspectFit,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Spacing = 0,
                        Children =
                        {
                            new Label
                            {
                                Text = "Prepop ID:",
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                VerticalOptions = LayoutOptions.Start,
                FontSize = 12
                            },
                            this.PrepopNameLabel,
                            new Label
                            {
                                Text = "Form Name:",
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                VerticalOptions = LayoutOptions.Start,
                FontSize = 12
                            },
                            this.PrepopFormLabel
                        }
                    }
                }
            };
        }

        public Label PrepopNameLabel { get; set; }
        public Label PrepopFormLabel { get; set; }

    }
}
