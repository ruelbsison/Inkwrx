using INKWRX_Mobile.UI;
using System;
using System.Threading.Tasks;
using static INKWRXPhotoTools_Mobile.PhotoTools;

using Xamarin.Forms;
using INKWRXPhotoTools_Mobile;
using System.IO;
using INKWRX_Mobile.Util;

namespace INKWRX_Mobile.Views
{
    public partial class CameraPage : ContentPage
    {
        public TabletImageView TabletImageView;
        private string TransactionId;
        
        private bool SaveToGallery;

        private CameraFieldView CameraFieldView;
        private Label backButton;
        private Image takeButton;
        private Image previousImage;

        public enum Orientation
        {
            Portrait,
            LandscapeLeft,
            LandscapeRight
        }

        public Orientation CurrentOrientation { get; set; } 

        public CameraPage(TabletImageView tiv, string tansactionId, FormProcessor processor)
        {
            InitializeComponent();
            this.TabletImageView = tiv;
            this.TransactionId = tansactionId;
            this.FormProcessor = processor;
            this.CurrentOrientation = Orientation.Portrait;
            StackLayout stacklayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0
            };
            this.Content = stacklayout;

            this.CameraFieldView = new CameraFieldView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            stacklayout.Children.Add(this.CameraFieldView);

            StackLayout buttonLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = 80,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = new Color(245f / 255f, 189f / 255f, 71f / 255f, 255f / 255f)
            };
            stacklayout.Children.Add(buttonLayout);

            this.backButton = new Label
            {
                Text = "< Back",
                BackgroundColor = Color.Transparent,
                HeightRequest = 80,
                WidthRequest = 80,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            var labelTap = new TapGestureRecognizer();
            labelTap.Tapped += OnButtonClickedBack;
            buttonLayout.Children.Add(this.backButton);
            this.backButton.GestureRecognizers.Add(labelTap);
            this.takeButton = new Image
            {
                Source = FileImageSource.FromFile("bar_icon_camera.png"),
                WidthRequest = 80,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            
            var takeTap = new TapGestureRecognizer();
            takeTap.Tapped += OnButtonClickedTake;
            this.takeButton.GestureRecognizers.Add(takeTap);
            buttonLayout.Children.Add(this.takeButton);

            var saveStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 80,
                WidthRequest = 80,
                Padding = new Thickness(3)
            };

            Switch saveSwitch = new Switch
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0)
            };
            //saveSwitch.Text = "Save To Gallery";//TODO add text to Switch
            saveSwitch.Toggled += (sender, eventArgs) =>
            {
                SaveToGallery = eventArgs.Value;
            };
            
            saveStack.Children.Add(saveSwitch);

            var saveLabel = new Label
            {
                Text = string.Format("Save to\n{0}", Device.OnPlatform("Camera Roll", "Gallery", "Documents")),
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 12,
                Margin = new Thickness(0, -5, 0, 5)
            };
            saveStack.Children.Add(saveLabel);
            var tapSave = new TapGestureRecognizer();
            tapSave.Tapped += (sender, eventArgs) =>
            {
                saveSwitch.IsToggled = !saveSwitch.IsToggled;
            };

            saveStack.GestureRecognizers.Add(tapSave);

            buttonLayout.Children.Add(saveStack);
            this.previousImage = new Image
            {
                WidthRequest = 70,
                HeightRequest = 70,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(5),
                Source = CoreAppTools.GetImageSource("bar_icon_gallery.png")
            };
            buttonLayout.Children.Add(this.previousImage);

            this.Content = stacklayout;
        }

        void OnButtonClickedBack(object sender, EventArgs args)
        {
            //((Button) sender).IsEnabled = false;
            //FormProcessor.addImageEntrys(ImageEntryList);//TODO when FormProcessor created
            goBack();
        }

        //hardware back button
        protected override bool OnBackButtonPressed()
        {
            goBack();

            return true;
        }

        private async void goBack()
        {
            if (this.takingPicture || this.takingThenSavingPictureCount != 0)
            {
                return;
            }
            await App.Current.MainPage.Navigation.PopModalAsync(false);
        }

        void OnButtonClickedTake(object sender, EventArgs args)
        {
            if (this.takingPicture)
            {
                return;
            }
            this.TakingPicture = true;
            this.TakingThenSavingPicture ++;
            this.CameraFieldView.TakePicture(this);
        }

        //if TabletImageView return false, else return true
        public async Task<bool> OnPictureTaken(byte[] imageData)
        {
            if (imageData != null)
            {
                ImageEntry imageEntry = new ImageEntry
                {
                    ImageType = SaveToGallery ? ImageEntry.ImageEntryType.Gallery : ImageEntry.ImageEntryType.Camera
                };
                
                imageEntry = await DependencyService.Get<IPhotoFileSystem>().SaveImage(imageData, imageEntry, TransactionId);
                this.FormProcessor.AttachedImages.Add(imageEntry);
                
                if (this.TabletImageView != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TabletImageView.AttachedImage = imageEntry;
                        this.TakingPicture = false;
                        this.TakingThenSavingPicture --;
                        this.goBack();
                    });
                    return false;
                }

                byte[] imageDataPrevious;
                if (Device.OnPlatform(false, true, false))
                {
                    imageDataPrevious = await DependencyService.Get<IPhotoFileSystem>().GetImage(imageEntry, 70, 70);
                }
                else
                {
                    imageDataPrevious = imageData;
                }
                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageDataPrevious));
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.previousImage.Source = imageSource;
                });
            }
            else if (this.TabletImageView != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.TakingPicture = false;
                    this.TakingThenSavingPicture --;
                    this.goBack();
                });
                return false;
            }
            if(Device.OnPlatform(true, false, true))
            {
                this.TakingPicture = false;
            }
            this.TakingThenSavingPicture --;
            return true;
        }
        public FormProcessor FormProcessor { get; private set; }
        private bool takingPicture = false;//set to false when picture is successfully taken in platform CameraView
        public bool TakingPicture
        {
            get
            {
                return this.takingPicture;
            }
            set
            {
                this.takingPicture = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.takeButton.Opacity = this.takingPicture ? 0d : 1d;
                });
            }
        }
        private int takingThenSavingPictureCount = 0;
        public int TakingThenSavingPicture
        {
            get
            {
                return this.takingThenSavingPictureCount;
            }
            set
            {
                this.takingThenSavingPictureCount = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.backButton.Opacity = this.takingThenSavingPictureCount == 0 ? 1d : 0d;
                });
            }
        }
    }
}