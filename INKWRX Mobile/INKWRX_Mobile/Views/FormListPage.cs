
using INKWRX_Mobile.Views.PageModels;
using INKWRX_Mobile.Connect.Types;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.Util;
using INKWRX_Mobile.Views.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using INKWRX_Mobile.Ui;

namespace INKWRX_Mobile.Views
{
    public class FormListPage : InkwrxBasePage
    {
        public FormListPage(InkwrxBasePage parent) : base("Forms", "Backgrounds/FormScreen/iw_app_ios_background_form.png", parent)
        {
            this.CurrentFolder = null;
            this.FormListItems = new ObservableCollection<FormListModel>();
            
            
            this.SearchEntry = new Entry
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Placeholder = "Search..."
            };

            this.SearchLabel = new Label
            {
                TextColor = CoreAppTools.SteelBlue,
                Text = "Search",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            var searchTap = new TapGestureRecognizer();
            searchTap.Tapped += async (sender, eventArgs) =>
            {
                if (this.AddFolderMode)
                {
                    if (!string.IsNullOrEmpty(this.SearchEntry.Text) && this.SearchEntry.Text.Trim() != "")
                    {
                        List<Folder> existing = null;
                        if (this.FolderId == -1)
                        {
                            existing = await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser);
                        }
                        else
                        {
                            existing = await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser, this.CurrentFolder);
                        }
                        var exists = existing.Any(f => f.Name == this.SearchEntry.Text.Trim());
                        if (exists)
                        {
                            await this.DisplayAlert("Folder Exists", "A folder with that name already exists", "Ok");
                        }
                        else
                        {
                            var folder = await App.DatabaseHelper.CreateFolderAsync(((App)App.Current).LoggedInUser, this.SearchEntry.Text.Trim(), this.CurrentFolder);
                            this.AddFolderMode = false;
                            this.RefreshForms(this.SearchEntry.Text);
                        }

                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    this.RefreshForms(this.SearchEntry.Text);
                }
            };
            this.SearchLabel.GestureRecognizers.Add(searchTap);

            this.SearchStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = CoreAppTools.LightSilver,
                Padding = 5,
                Children =
                {
                    this.SearchEntry,
                    this.SearchLabel
                }
            };

            Button showAll = new Button
            {
                Text = "All",
                BackgroundColor = CoreAppTools.SteelBlue,
                TextColor = Color.White,
				WidthRequest = 100
            };
            Button showForms = new Button
            {
                Text = "Forms",
                BackgroundColor = Color.White,
                TextColor = CoreAppTools.SteelBlue,
				WidthRequest = 100
            };
            Button showFolders = new Button
            {
                Text = "Folders",
                BackgroundColor = Color.White,
                TextColor = CoreAppTools.SteelBlue,
				WidthRequest = 100
            };
            showAll.Clicked += ((sender, eventArgs) => {
                if (this.Viewing == ViewType.Both)
                {
                    return;
                }
                showAll.BackgroundColor = CoreAppTools.SteelBlue;
                showForms.BackgroundColor = Color.White;
                showFolders.BackgroundColor = Color.White;
                showAll.TextColor = Color.White;
                showForms.TextColor = CoreAppTools.SteelBlue;
                showFolders.TextColor = CoreAppTools.SteelBlue;
                this.Viewing = ViewType.Both;
                this.RefreshForms(this.SearchEntry.Text);
                
            });
            showForms.Clicked += ((sender, eventArgs) => {
                if (this.Viewing == ViewType.Forms)
                {
                    return;
                }
                showAll.BackgroundColor = Color.White;
                showForms.BackgroundColor = CoreAppTools.SteelBlue;
                showFolders.BackgroundColor = Color.White;
                showAll.TextColor = CoreAppTools.SteelBlue;
                showForms.TextColor = Color.White;
                showFolders.TextColor = CoreAppTools.SteelBlue;
                this.Viewing = ViewType.Forms;
                this.RefreshForms(this.SearchEntry.Text);
            });
            showFolders.Clicked += ((sender, eventArgs) => {
                if (this.Viewing == ViewType.Folders)
                {
                    return;
                }
                showAll.BackgroundColor = Color.White;
                showForms.BackgroundColor = Color.White;
                showFolders.BackgroundColor = CoreAppTools.SteelBlue;
                showAll.TextColor = CoreAppTools.SteelBlue;
                showForms.TextColor = CoreAppTools.SteelBlue;
                showFolders.TextColor = Color.White;
                this.Viewing = ViewType.Folders;
                this.RefreshForms(this.SearchEntry.Text);
            });

            this.RefreshButton = new Image
            {
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/NavBar/iw_app_ios_navbar_icon_refresh.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            var addFolderButton = new Image
            {
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/NavBar/iw_app_ios_navbar_icon_addfolder.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            this.RemoveFolderButton = new Image
            {
                Source = CoreAppTools.GetImageSource("Icons/FormScreen/NavBar/iw_app_ios_navbar_icon_deletefolder.png"),
                HeightRequest = 25,
                WidthRequest = 25,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            var addTap = new TapGestureRecognizer();
            addTap.Tapped += async (sender, eventArgs) =>
            {
                if (this.TappedItem == null)
                {
                    if (this.AddFolderMode)
                    {
                        this.AddFolderMode = false;
                    }
                    else
                    {
                        this.AddFolderMode = true;
                    }
                    return;
                }
                
                if (this.AddFolderMode)
                {
                    this.AddFolderMode = false;
                    return;
                }

                var folderList = await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser, this.CurrentFolder);
                
                if (this.TappedItem.FolderItem != null)
                {
                    var tapped = folderList.FirstOrDefault(x => x.Id == this.TappedItem.FolderItem.Id);
                    if (tapped != null)
                    {
                        folderList.Remove(tapped);
                    }
                }

                if (!folderList.Any() && this.CurrentFolder == null)
                {
                    this.AddFolderMode = true;
                    return;
                }
                

                var action1 = await this.DisplayActionSheet("What would you like to do?", "Cancel", null, "Create Folder", "Move To Folder");
                switch (action1)
                {
                    case "Create Folder":
                        this.AddFolderMode = true;
                        return;
                    case "Move To Folder":
                        var folderStrings = folderList.Select(x => x.Name).ToList();
                        if (this.CurrentFolder != null)
                        {
                            folderStrings.Insert(0, "Move Up "); // Space added intentionally - folder name is trimmed, so this can allow a folder named "Move Up"
                        }
                        var action2 = await this.DisplayActionSheet("Move to...", "Cancel", null, folderStrings.ToArray());
                        if (action2 == "Cancel")
                        {
                            return;
                        }
                        int folderId = -1;
                        if (action2 == "Move Up ") // Space added intentionally - see above
                        {
                            folderId = this.CurrentFolder.Parent;
                        }
                        else
                        {
                            var folder = folderList.FirstOrDefault(x => x.Name == action2);
                            if (folder == null)
                            {
                                return;
                            }
                            folderId = folder.Id;
                        }
                        
                        if (this.TappedItem.FormItem == null)
                        {
                            //folder
                            this.TappedItem.FolderItem.Parent = folderId;
                            await App.DatabaseHelper.UpdateItemAsync(this.TappedItem.FolderItem);
                        }
                        else
                        {
                            //form
                            this.TappedItem.FormItem.ParentFolder = folderId;
                            await App.DatabaseHelper.UpdateItemAsync(this.TappedItem.FormItem);
                        }
                        this.TappedItem = null;
                        this.RefreshButton.IsVisible = false;
                        this.RemoveFolderButton.IsVisible = false;
                        this.RefreshForms(this.SearchEntry.Text);

                        return;
                }

            };
            addFolderButton.GestureRecognizers.Add(addTap);

            var removeTap = new TapGestureRecognizer();
            removeTap.Tapped += async (sender, eventArgs) =>
            {
                var folder = this.TappedItem.FolderItem;
                var response = await this.DisplayAlert("Confirm", string.Format("Are you sure you want to delete the folder named\n\"{0}\"\nThis will move all forms to the current folder.", folder.Name), "Yes", "No");
                if (response)
                {
                    await App.DatabaseHelper.DeleteFolderAsync(folder);
                    this.TappedItem = null;
                    this.RefreshButton.IsVisible = false;
                    this.RemoveFolderButton.IsVisible = false;
                    this.RefreshForms(this.SearchEntry.Text);
                }
            };
            this.RemoveFolderButton.GestureRecognizers.Add(removeTap);

            this.RefreshButton.IsVisible = false;
            this.RemoveFolderButton.IsVisible = false;

            this.RightButtons.Children.Add(this.RefreshButton);
            this.RightButtons.Children.Add(this.RemoveFolderButton);
            this.RightButtons.Children.Add(addFolderButton);


			var showBarStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HeightRequest = 40,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = CoreAppTools.LightSilver,
				Children =
				{
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HeightRequest = 40,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Spacing = Device.OnPlatform(5,5,0),
						Padding = Device.OnPlatform(5,6,0),
                        Children =
                        {
                            showAll,
                            showForms,
                            showFolders
                        }
                    }
                }
            };
            
            
            this.FormListView = new FormListView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = new DataTemplate(typeof(FormListItemView)),
                ItemsSource = this.FormListItems,
				HasUnevenRows = true,
                RowHeight = 60
            };
            
            this.FormListView.ItemTapped += (sender, eventArgs) =>
            {
                var item = (FormListModel)eventArgs.Item;
                if (item == this.TappedItem)
                {
                    
                    // second tap
                    if (item.FolderItem != null)
                    {
                        //folder
                        this.CurrentFolder = item.FolderItem;
                        this.TappedItem = null;
                        this.AddFolderMode = false;
                        this.FormListView.SelectedItem = null;
                        this.RefreshButton.IsVisible = false;
                        this.RemoveFolderButton.IsVisible = false;
                        this.RefreshForms(this.SearchEntry.Text);
                    }
                    else
                    {
                        //form
                        this.TappedItem = null;
                        this.AddFolderMode = false;
                        this.FormListView.SelectedItem = null;
                        this.RefreshButton.IsVisible = false;
                        this.RemoveFolderButton.IsVisible = false;
                        App.Current.MainPage = new FormViewPage(this, item.FormItem);
                    }
                }
                else
                {
                    this.TappedItem = item;
                    if (item.FolderItem != null)
                    {
                        //folder selected
                        this.RemoveFolderButton.IsVisible = true;
                        this.RefreshButton.IsVisible = false;
                    }
                    else
                    {
                        //form selected
                        this.RemoveFolderButton.IsVisible = false;
                        this.RefreshButton.IsVisible = true;
                    }
                }
            };
            this.PageContent.Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    this.SearchStack,
                    showBarStack,
                    this.FormListView
                }
            };

            this.RefreshForms("");
        }

        protected override async void GoBack()
        {
            if (this.FolderId == -1)
            {
                base.GoBack();
            }
            else
            {
                var parent = this.CurrentFolder.Parent;
                if (parent == -1)
                {
                    this.CurrentFolder = null;
                    this.TappedItem = null;
                    this.RefreshButton.IsVisible = false;
                    this.RemoveFolderButton.IsVisible = false;
                    this.RefreshForms(this.SearchEntry.Text);
                    return;
                }
                var parentFolder = (await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser)).FirstOrDefault(f => f.Id == parent);
                if (parentFolder == null)
                {
                    base.GoBack();
                    return;
                }
                this.TappedItem = null;
                this.RefreshButton.IsVisible = false;
                this.RemoveFolderButton.IsVisible = false;
                this.CurrentFolder = parentFolder;
                this.RefreshForms(this.SearchEntry.Text);
            }
        }

        public void RefreshForms(string searchText)
        {
            Task.Run(async () =>
            {
                List<Form> forms = null;
                List<Folder> folders = null;
                string headerText;
                if (this.FolderId == -1)
                {
                    headerText = "Forms";
                    forms = await App.DatabaseHelper.GetFormsAsync(((App)App.Current).LoggedInUser);
                    folders = await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser);
                }
                else
                {
                    headerText = this.CurrentFolder.Name;
                    forms = await App.DatabaseHelper.GetFormsAsync(((App)App.Current).LoggedInUser, this.CurrentFolder);
                    folders = await App.DatabaseHelper.GetFoldersAsync(((App)App.Current).LoggedInUser, this.CurrentFolder);
                }

                var items = new List<FormListModel>();


                if (!string.IsNullOrEmpty(searchText))
                {
                    forms = forms.Where(f => f.FormName.ToLower().Contains(searchText.ToLower())).ToList();
                    folders = folders.Where(f => f.Name.ToLower().Contains(searchText.ToLower())).ToList();
                }

                if (this.Viewing == ViewType.Both || this.Viewing == ViewType.Folders)
                {
                    items.AddRange(folders.Select(f => new FormListModel(f)).ToList());
                }
                if (this.Viewing == ViewType.Both || this.Viewing == ViewType.Forms)
                {
                    items.AddRange(forms.Select(f => new FormListModel(f)).ToList());
                }

                this.FormListItems = new ObservableCollection<FormListModel>(items);
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.HeaderLabel.Text = headerText;
                    this.FormListView.ItemsSource = this.FormListItems;
                });
            });
        }

        private ViewType Viewing = ViewType.Both;

        private enum ViewType
        {
            Both,
            Forms,
            Folders
        }
        private FormListModel tappedItem = null;
        private FormListModel TappedItem
        {
            get
            {
                return this.tappedItem;
            }
            set
            {
                if (this.tappedItem != null)
                {
                    this.tappedItem.Selected = false;
                }
                if (value != null)
                {
                    value.Selected = true;
                }
                this.tappedItem = value;
            }
        }

        private bool addFolderMode = false;

        public bool AddFolderMode
        {
            get
            {
                return this.addFolderMode;
            }
            set
            {
                this.addFolderMode = value;
                if (this.addFolderMode)
                {
                    this.SearchLabel.Text = "Create";
                    this.SearchEntry.Placeholder = "New Folder Name...";
                    this.SearchEntry.Text = "";
                    this.SearchEntry.Focus();
                }
                else
                {
                    this.SearchLabel.Text = "Search";
                    this.SearchEntry.Placeholder = "Search...";
                    this.SearchEntry.Text = "";
                }
            }
        }

        private bool moveToFolderMode = false;

        public bool MoveToFolderMode
        {
            get
            {
                return this.moveToFolderMode;
            }
            set
            {
                this.moveToFolderMode = true;
            }
        }

        public Image RefreshButton { get; set; }
        public Image RemoveFolderButton { get; set; }
        public Label SearchLabel { get; set; }
        public Entry SearchEntry { get; set; }
        public StackLayout SearchStack { get; set; }
        public List<Folder> Folders { get; set; }
        public ObservableCollection<FormListModel> FormListItems { get; set; }

        public int FolderId {
            get
            {
                return this.CurrentFolder == null ? -1 : this.CurrentFolder.Id;
            }
        }
        public Folder CurrentFolder { get; set; }

        public FormListView FormListView { get; set; }
    }
}
