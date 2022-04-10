using INKWRX_Mobile.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class HistoryItemView : ViewCell
    {
        public HistoryItemView()
        {
            var image = new Image
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = 40,
                HeightRequest = 40,
                Aspect = Aspect.AspectFit
            };
            image.SetBinding(Image.SourceProperty, "IconSource");
            var formName = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                FontSize = 12
            };
            formName.SetBinding(Label.TextProperty, "FormName");

            var startedDate = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Gray,
                FontSize = 12
            };

            var savedDate = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Gray,
                FontSize = 12
            };

            var sentDate = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Gray,
                FontSize = 12
            };

            startedDate.SetBinding(Label.TextProperty, "StartedLabel");
            savedDate.SetBinding(Label.TextProperty, "SavedLabel");
            sentDate.SetBinding(Label.TextProperty, "SentLabel");
            
            Label arrow = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Gray,
                Text = ">"
            };
            arrow.SetBinding(VisualElement.IsVisibleProperty, "ArrowIsVisible");
            View = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 5, 10, 5),
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = {
                    image,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Padding = new Thickness(10, 5, 10, 5),
                        Children = {
                            formName,
                            startedDate,
                            savedDate,
                            sentDate
                        }
                    },
                    arrow
                }
            };
        }
    }
}