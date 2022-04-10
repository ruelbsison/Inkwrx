using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views.PageModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.Views
{
    public class PrepopPage : InkwrxBasePage
    {
        public PrepopPage(InkwrxBasePage parent) : base ("Prepop", "Backgrounds/FormScreen/iw_app_ios_background_form.png", parent)
        {
            this.PrepopItems = new ObservableCollection<PrepopItemModel>();
            var mainStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            this.SearchStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = CoreAppTools.LightSilver,
                Padding = 5
            };

            this.SearchEntry = new Entry
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Placeholder = "Search..."
            };

            var searchLabel = new Label
            {
                TextColor = CoreAppTools.SteelBlue,
                Text = "Search",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            var searchTap = new TapGestureRecognizer();
            searchTap.Tapped += (sender, eventArgs) =>
            {
                this.RefreshPrepops(this.SearchEntry.Text);
            };
            searchLabel.GestureRecognizers.Add(searchTap);

            this.SearchStack.Children.Add(this.SearchEntry);
            this.SearchStack.Children.Add(searchLabel);

            this.PrepopList = new ListView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = new DataTemplate(typeof(PrepopItem)),
                SeparatorColor = Color.Black,
                SeparatorVisibility = SeparatorVisibility.Default,
                ItemsSource = this.PrepopItems,
				HasUnevenRows = true
            };

            this.PrepopList.ItemTapped += PrepopListItemTapped;

            mainStack.Children.Add(this.SearchStack);
            mainStack.Children.Add(this.PrepopList);

            this.PageContent.Content = mainStack;
        }

        private async void PrepopListItemTapped(object sender, ItemTappedEventArgs eventArgs)
        {
            this.PrepopList.SelectedItem = null;
            var prepopItem = (PrepopItemModel)eventArgs.Item;
            var form = await App.DatabaseHelper.GetFormAsync(prepopItem.PrepopForm.Form);
            if (form == null)
            {
                return;
            }
            App.Current.MainPage = new FormViewPage(this, form, null, prepopItem.PrepopForm);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.RefreshPrepops(this.SearchEntry.Text);
        }

        private void RefreshPrepops(string searchString = "")
        {
            Task.Run(async () =>
            {
                var items = await App.DatabaseHelper.GetPrepopForms(((App)App.Current).LoggedInUser);
                if (string.IsNullOrEmpty(searchString))
                {
                    // all items
                    this.PrepopItems = new ObservableCollection<PrepopItemModel>(items.Select(x => new PrepopItemModel(x)));
                }
                else
                {
                    // search items
                    this.PrepopItems = new ObservableCollection<PrepopItemModel>(items.Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                        .Select(x => new PrepopItemModel(x)));
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.PrepopList.ItemsSource = this.PrepopItems;
                });
            });
        }

        public ObservableCollection<PrepopItemModel> PrepopItems { get; set; }

        public StackLayout SearchStack { get; set; }
        public ListView PrepopList { get; set; }
        public Entry SearchEntry { get; set; }
    }
}
