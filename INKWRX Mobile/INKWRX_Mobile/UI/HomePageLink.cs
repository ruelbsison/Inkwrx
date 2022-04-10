using INKWRX_Mobile.Views.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class HomePageLink : ViewCell
    {
        public HomePageLink ()
        {
            var mainLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = 50,
                Padding = new Thickness(10, 5, 10, 5)
            };

            var imageIcon = new Image
            {
                WidthRequest = 40,
                HeightRequest = 40,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            imageIcon.SetBinding(Image.SourceProperty, "IconSource");
            mainLayout.Children.Add(imageIcon);

            var label = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            label.SetBinding(Label.TextProperty, "Label");
            mainLayout.Children.Add(label);

            var indicator = new Label
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            indicator.SetBinding(Label.TextProperty, "Indicator");
            mainLayout.Children.Add(indicator);

            var more = new Label
            {
                Text = ">",
                TextColor = Color.Gray,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            var visBinding = new Binding
            {
                Path = "CanView",
                Mode = BindingMode.OneWay,
                Converter = new HomeBoolToOpacityValueConverter()
            };
            more.SetBinding(Label.OpacityProperty, visBinding);
            mainLayout.Children.Add(more);

            this.View = mainLayout;
            this.View.BackgroundColor = Color.White;
        }
    }
}
