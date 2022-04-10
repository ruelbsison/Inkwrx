using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Dependencies;
using INKWRX_Mobile.Views.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamvvm;

namespace INKWRX_Mobile.Views.UI
{
    public class FormListItemView : ViewCell
    {
        public FormListItemView()
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
            
            Label formName = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black
            };
            formName.SetBinding(Label.TextProperty, "ItemName");

            Label arrow = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Gray,
                Text = ">"
            };

            var stack = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 50,
                Padding = new Thickness(10, 5, 10, 5)
            };

            stack.RowDefinitions.Add(new RowDefinition
            {
                Height = 50
            });
            stack.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = 50
            });
            stack.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            stack.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Auto)
            });

            stack.Children.Add(image, 0, 1, 0, 1);
            stack.Children.Add(formName, 1, 2, 0, 1);
            stack.Children.Add(arrow, 2, 3, 0, 1);

            this.View = stack;

            this.View.BackgroundColor = Color.White;
            // adds custom colour, regardless of OS
            this.View.SetBinding(VisualElement.BackgroundColorProperty, new Binding("BackgroundColor"));
        }
    }
}