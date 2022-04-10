using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static INKWRX_Mobile.Views.PageModels.HomePageLinkModel;
using Xamarin.Forms;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.UI;
using System.Collections.ObjectModel;
using INKWRX_Mobile.Views.PageModels;
using INKWRX_Mobile.Database;

namespace INKWRX_Mobile.Views
{
    public class HistoryPage : InkwrxBasePage
    {
        public HistoryPage(PageType type, InkwrxBasePage parentPage) : base("History", "Backgrounds/HistoryScreen/iw_app_ios_background_history.png", parentPage)
        {
            this.HeaderLabel.Text = PageTitles[type];
            this.HeaderBackground.Source = CoreAppTools.GetImageSource(PageBackgrounds[type]);
            this.HistoryItems = new ObservableCollection<HistoryItemModel>();
            this.PageType = type;
            this.HomeButton = new Image
            {
                HeightRequest = 25,
                WidthRequest = 25,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/HistoryScreen/NavBar/iw_app_ios_navbar_icon_home.png"),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            TapGestureRecognizer homeTapped = new TapGestureRecognizer();
            homeTapped.Tapped += (sender, eventArgs) => {
                App.Current.MainPage = this.ParentPage;
            };
            this.HomeButton.GestureRecognizers.Add(homeTapped);

            this.SortButton = new Image
            {
                HeightRequest = 25,
                WidthRequest = 25,
                Aspect = Aspect.AspectFit,
                Source = CoreAppTools.GetImageSource("Icons/HistoryScreen/NavBar/iw_app_ios_icon_sort.png"),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            TapGestureRecognizer filterTapped = new TapGestureRecognizer();
            filterTapped.Tapped += async (sender, eventArgs) => {
                var options = new List<string>
                {
                    "Ascending",
                    "Descending",
                    "Form Name"
                };
                if (this.PageType == PageType.HistoryAll || this.PageType == PageType.HistorySent)
                {
                    options.Add("Sent Date");
                }
                options.Add("Saved Date");
                options.Add("Started Date");
                string action = await DisplayActionSheet("Sort By:", "Cancel", null, options.ToArray());
                //TODO process action
                switch (action)
                {
                    case "Ascending":
                        this.SortDescending = false;
                        break;
                    case "Descending":
                        this.SortDescending = true;
                        break;
                    case "Form Name":
                        this.SortType = SortOptions.FormName;
                        break;
                    case "Sent Date":
                        this.SortType = SortOptions.SentDate;
                        break;
                    case "Saved Date":
                        this.SortType = SortOptions.ParkedDate;
                        break;
                    case "Started Date":
                        this.SortType = SortOptions.StartedDate;
                        break;

                    case "Cancel":
                        return;
                }

                this.RefreshHistory();
            };
            this.SortButton.GestureRecognizers.Add(filterTapped);

            this.RightButtons.Children.Add(this.HomeButton);
            this.RightButtons.Children.Add(this.SortButton);

			this.HistoryListView = new ListView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				ItemTemplate = new DataTemplate(typeof(HistoryItemView)),
				ItemsSource = this.HistoryItems,
				SeparatorVisibility = SeparatorVisibility.Default,
				SeparatorColor = Color.Black,
				HasUnevenRows = true
            };
            
            this.HistoryListView.ItemTapped += async (sender, eventArgs) =>
            {
                var selectedItem = (HistoryItemModel)eventArgs.Item;
                if (selectedItem.ArrowIsVisible)
                {
                    Form form = await App.DatabaseHelper.GetFormAsync(selectedItem.Transaction.Form);
                    if (form.Status == (int)DatabaseHelper.FormStatus.Deleted)
                    {
                        await this.DisplayAlert("Form Removed", "The form associated with this transaction has been removed from the form list. To access this transaction, the form must be moved out of Design Status", "Ok");

                        return;
                    }
                    if (selectedItem.Transaction.Status == (int)DatabaseHelper.Status.Parked)
                    {
                        Transaction autosave = await App.DatabaseHelper.GetAutosaveFromOriginal(selectedItem.Transaction.Id);
                        if (autosave != null && selectedItem.Transaction.SavedDate < autosave.SavedDate)
                        {
                            if (await DisplayAlert("Are You Sure?",
                                "An auto-saved version of the form has been recovered with newer data.\nAre you sure you wish to abandon the recovered form and open the older parked version?",
                                "Open Parked",
                                "Cancel"))
                            {
                                await App.DatabaseHelper.DeleteTransactionAsync(autosave);
                                var formPage = new FormViewPage(this, form, selectedItem.Transaction);
                                App.Current.MainPage = formPage;
                            }
                        }
                        else
                        {
                            if (autosave != null)
                            {
                                await App.DatabaseHelper.DeleteTransactionAsync(autosave);
                            }
                            var formPage = new FormViewPage(this, form, selectedItem.Transaction);
                            App.Current.MainPage = formPage;
                        }
                    }
                    else
                    {
                        var formPage = new FormViewPage(this, form, selectedItem.Transaction);
                        App.Current.MainPage = formPage;
                    }
                }
                this.HistoryListView.SelectedItem = null;
            };
            this.PageContent.Content = this.HistoryListView;

            this.RefreshHistory();
        }

        public ObservableCollection<HistoryItemModel> HistoryItems { get; set; }

        public static Dictionary<PageType, string> PageTitles = new Dictionary<PageType, string>
        {
            { PageType.HistoryAll, "History" },
            { PageType.HistoryParked, "Parked" },
            { PageType.HistoryPending, "Pending" },
            { PageType.HistorySent, "Sent" },
            { PageType.HistoryAutosaved, "Auto Save" }
        };

        public static Dictionary<PageType, string> PageBackgrounds = new Dictionary<PageType, string>
        {
            { PageType.HistoryAll, "Backgrounds/HistoryScreen/iw_app_ios_background_history.png" },
            { PageType.HistoryParked, "Backgrounds/ParkedScreen/iw_app_ios_background_parked.png" },
            { PageType.HistoryPending, "Backgrounds/PendingScreen/iw_app_ios_background_pending.png" },
            { PageType.HistorySent, "Backgrounds/SentScreen/iw_app_ios_background_sent.png" },
            { PageType.HistoryAutosaved, "Backgrounds/AutoSaveScreen/iw_app_ios_background_autosave.png" }
        };

        public void RefreshHistory()
        {
            Task.Run(async () =>
            {
                List<Transaction> items = null;
                switch (this.PageType)
                {
                    case PageType.HistoryAutosaved:
                        items = await App.DatabaseHelper.GetTransactionsAsync(((App)App.Current).LoggedInUser, Database.DatabaseHelper.Status.Autosaved);
                        break;
                    case PageType.HistorySent:
                        items = await App.DatabaseHelper.GetTransactionsAsync(((App)App.Current).LoggedInUser, Database.DatabaseHelper.Status.Sent);
                        break;
                    case PageType.HistoryPending:
                        items = await App.DatabaseHelper.GetTransactionsAsync(((App)App.Current).LoggedInUser, Database.DatabaseHelper.Status.Pending);
                        break;
                    case PageType.HistoryParked:
                        items = await App.DatabaseHelper.GetTransactionsAsync(((App)App.Current).LoggedInUser, Database.DatabaseHelper.Status.Parked);
                        break;
                    default:
                        items = await App.DatabaseHelper.GetTransactionsAsync(((App)App.Current).LoggedInUser);
                        break;
                }

                switch (this.SortType)
                {
                    case SortOptions.FormName:
                        items = this.SortDescending
                            ? items.OrderByDescending(x => x.FormName).ToList()
                            : items.OrderBy(x => x.FormName).ToList();
                        break;
                    case SortOptions.ParkedDate:
                        items = this.SortDescending
                            ? items.OrderByDescending(x => x.SavedDate).ToList()
                            : items.OrderBy(x => x.SavedDate).ToList();
                        break;
                    case SortOptions.SentDate:
                        items = this.SortDescending
                            ? items.OrderByDescending(x => x.SentDate).ToList()
                            : items.OrderBy(x => x.SentDate).ToList();
                        break;
                    case SortOptions.StartedDate:
                        items = this.SortDescending
                            ? items.OrderByDescending(x => x.StartedDate).ToList()
                            : items.OrderBy(x => x.StartedDate).ToList();
                        break;
                }

                this.HistoryItems = new ObservableCollection<HistoryItemModel>(items.Select(t => new HistoryItemModel(t)).ToList());
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.HistoryListView.ItemsSource = this.HistoryItems;
                });
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.RefreshHistory();
        }

        private enum SortOptions
        {
            FormName,
            SentDate,
            StartedDate,
            ParkedDate
        }

        private SortOptions SortType = SortOptions.StartedDate;

        private bool SortDescending = true;
                
        public Image HomeButton { get; set; }
        public Image SortButton { get; set; }
        public PageType PageType { get; set; }
        private ListView HistoryListView;
    }
}
