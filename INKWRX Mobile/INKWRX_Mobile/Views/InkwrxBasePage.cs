using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace INKWRX_Mobile.Views
{
    public class InkwrxBasePage : ContentPage
    {
        public InkwrxBasePage(string pageTitle, string headerBgPath, InkwrxBasePage parentPage)
        {
            this.ParentPage = parentPage;
            var mainLayout = new StackLayout
            {
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0
            };
            

            this.NavBarRel = new RelativeLayout
            {
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = Device.OnPlatform(70,50,50),
                HorizontalOptions = LayoutOptions.Fill
            };
            this.HeaderLabel = new Label
            {
                Text = pageTitle,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White
            };

            this.HeaderBackground = new Image
            {
                Aspect = Aspect.Fill,
                Source = CoreAppTools.GetImageSource(headerBgPath)
            };
            this.NavBarRel.Children.Add(this.HeaderBackground, 
                Constraint.Constant(0), 
                Constraint.Constant(0), 
                Constraint.RelativeToParent((parent) => { return parent.Width; }), 
                Constraint.RelativeToParent((parent) => { return parent.Height; })
            );

            this.NavBarRel.Children.Add(this.HeaderLabel,
               Constraint.Constant(0),
               Constraint.Constant(Device.OnPlatform(20,0,0)),
               Constraint.RelativeToParent((parent) => { return parent.Width; }),
               Constraint.RelativeToParent((parent) => { return parent.Height - Device.OnPlatform(20,0,0); })
           );

            this.NavBarStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(20, Device.OnPlatform(20, 0, 0), 20, 0)
            };
            this.NavBarRel.Children.Add(this.NavBarStack,
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToParent((parent) => { return parent.Height; })
            );

            this.LeftButtons = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal
            };
            this.RightButtons = new StackLayout
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Spacing = 25
            };

            this.NavBarStack.Children.Add(this.LeftButtons);
            this.NavBarStack.Children.Add(this.RightButtons);
            this.BackButton = new Label
            {
                TextColor = Color.White,
                Text = "< Back",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            var backButtonTapped = new TapGestureRecognizer();
            backButtonTapped.Tapped += this.BackButtonTapped;
            this.BackButton.GestureRecognizers.Add(backButtonTapped);
            this.LeftButtons.Children.Add(this.BackButton);

            this.PageContent = new ContentView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            this.ToolbarStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = CoreAppTools.LightSilver,
                Padding = new Thickness(5, 5, 5, 5),
                HeightRequest = 50,
                IsVisible = false
            };


            mainLayout.Children.Add(this.NavBarRel);
            mainLayout.Children.Add(this.PageContent);
            mainLayout.Children.Add(this.ToolbarStack);
            this.Content = mainLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(!(this is FormViewPage))
            {
                this.PageLoading = false;
            }
        }

        protected void BackButtonTapped(object sender, EventArgs eventArgs)
        {
            this.GoBack();
        }

        //hardware back button
        protected override bool OnBackButtonPressed()
        {
            if (this.ParentPage == null)
            {
                return false;
            }

            this.GoBack();

            return true;
        }

        protected virtual void GoBack()
        {
            if (this.PageLoading)
            {
                return;
            }
            this.PageLoading = true;

            this.ParentPage.PageLoading = true;
            App.Current.MainPage = this.ParentPage;
            if (App.Current.MainPage is HomePage)
            {
                ((HomePage)App.Current.MainPage).HomeTable.SelectedItem = null;
            }
            else if (App.Current.MainPage is HistoryPage)
            {
                ((HistoryPage)App.Current.MainPage).RefreshHistory();
            }
        }

        public InkwrxBasePage ParentPage { get; set; }
        public RelativeLayout NavBarRel { get; set; }
        public StackLayout NavBarStack { get; set; }
        public StackLayout LeftButtons { get; set; }
        public StackLayout RightButtons { get; set; }
        public Label HeaderLabel { get; set; }
        public Image HeaderBackground { get; set; }
        public ContentView PageContent { get; set; }
        public StackLayout ToolbarStack { get; set; }
        public Label BackButton { get; set; }

        protected bool PageLoading = true;
    }
}
