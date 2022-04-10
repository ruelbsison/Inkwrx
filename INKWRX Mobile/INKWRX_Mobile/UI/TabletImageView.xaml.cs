using FormTools.FormDescriptor;
using INKWRX_Mobile.Util;
using INKWRX_Mobile.Views;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static INKWRXPhotoTools_Mobile.PhotoTools;

namespace INKWRX_Mobile.UI
{
    public partial class TabletImageView : ContentView, IElementView
    {
        public TabletImageView()
        {
            InitializeComponent();
        }

        public TabletImageView(TabletImageDescriptor tid, FormProcessor processor) : this()
        {
            this.Descriptor = tid;
            this.HeightMultiplier = this.Descriptor.Height / this.Descriptor.Width;
            this.FormProcessor = processor;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            this.InnerRel.Children.Clear();
            this.ImageView = new Image();
            this.ClearButton = new Label();
            this.ClearButton.TextColor = Color.Red;
            this.ClearButton.Text = "X";
            this.ClearButton.Margin = 0;
            this.ClearButton.BackgroundColor = Color.Transparent;
            
            this.ButtonGrid = new Grid();
            this.ButtonGrid.BackgroundColor = new Color(245f/255f, 189f/255f, 71f/255f, 255f/255f);
            
            this.ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            this.ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var galleryImage = new Image();
            galleryImage.HorizontalOptions = LayoutOptions.CenterAndExpand;
            galleryImage.VerticalOptions = LayoutOptions.CenterAndExpand;
            galleryImage.Aspect = Aspect.AspectFit;
            galleryImage.Source = ImageSource.FromFile("bar_icon_gallery.png");
            var photoImage = new Image();
            photoImage.Aspect = Aspect.AspectFit;
            photoImage.HorizontalOptions = LayoutOptions.CenterAndExpand;
            photoImage.VerticalOptions = LayoutOptions.CenterAndExpand;
            photoImage.Source = ImageSource.FromFile("bar_icon_camera.png");
            this.ButtonGrid.Children.Add(galleryImage, 0, 1, 0, 1);
            this.ButtonGrid.Children.Add(photoImage, 1, 2, 0, 1);
            this.ButtonGrid.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.ButtonGrid.VerticalOptions = LayoutOptions.FillAndExpand;
            this.InnerRel.Children.Add(this.ButtonGrid, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            this.InnerRel.Children.Add(this.ImageView, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height; }));
            this.InnerRel.Children.Add(this.ClearButton, Constraint.RelativeToParent((parent) => { return parent.Width - 25; }), Constraint.Constant(0),
                Constraint.Constant(25), Constraint.Constant(25));

            var galleryTap = new TapGestureRecognizer();
            galleryTap.Tapped += (sender, eventArgs) =>
            {
                var formViewPage = ((FormViewPage)App.Current.MainPage);
                formViewPage.ScrollY = formViewPage.Scroller.ScrollY;
                App.Current.MainPage = new GalleryAttachPage((FormViewPage)App.Current.MainPage, this, this.FormProcessor);
            };
            galleryImage.GestureRecognizers.Add(galleryTap);

            var photoTap = new TapGestureRecognizer();
            photoTap.Tapped += async (sender, eventArgs) =>
            {
                var tranId = ((FormViewPage)App.Current.MainPage).Processor.CurrentTransaction == null
                    ? "-1"
                    : ((FormViewPage)App.Current.MainPage).Processor.CurrentTransaction.Id.ToString();
                var formViewPage = ((FormViewPage)App.Current.MainPage);
                formViewPage.ScrollY = formViewPage.Scroller.ScrollY;
                await App.Current.MainPage.Navigation.PushModalAsync(new CameraPage(this, tranId, this.FormProcessor));
            };
            photoImage.GestureRecognizers.Add(photoTap);

            this.AttachedImage = null;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (sender, eventArgs) =>
            {
                var others = this.FormProcessor.AllFields.OfType<TabletImageView>()
                                .Where(x => x.Descriptor.FieldId != this.Descriptor.FieldId 
                                            && x.AttachedImage != null 
                                            && x.AttachedImage.ImageReference == this.AttachedImage.ImageReference)
                                .Any();
                if (!others)
                {
                    var attached = this.FormProcessor.AttachedImages.FirstOrDefault(x => x.ImageReference == this.AttachedImage.ImageReference);
                    if (attached != null)
                    {
                        this.FormProcessor.AttachedImages.Remove(attached);
                    }
                }

                this.AttachedImage = null;
            };
            this.ClearButton.GestureRecognizers.Add(tap);
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var newSize = new SizeRequest(new Size(widthConstraint, widthConstraint * this.HeightMultiplier));
            
            return newSize;
        }

        public Image ImageView { get; set; }

        public Label ClearButton { get; set; }

        public Grid ButtonGrid { get; set; }

        public Color StrokeColour
        {
            get { return this.Descriptor.StrokeColour.ToColor(); }
        }

        public TabletImageDescriptor Descriptor { get; set; }

        private ImageEntry attachedImage = null;

        public event FieldValueChangedEventHandler FieldValueChanged;

        public ImageEntry AttachedImage {
            get
            {
                return attachedImage;
            }
            set
            {
                attachedImage = value;
                if (value == null)
                {
                    this.ClearButton.IsVisible = false;
                    this.ImageView.Source = null;
                    this.ImageView.IsVisible = false;
                    this.ButtonGrid.IsVisible = true;
                }
                else
                {
                    this.ClearButton.IsVisible = true;
                    this.ButtonGrid.IsVisible = false;
                    this.ImageView.IsVisible = true;
                    this.loadImageSource();
                }
                this.FieldValueChanged?.Invoke(this, new EventArgs());
            }
        }
        

        private async void loadImageSource ()
        {
            var bytes = await DependencyService.Get<IPhotoFileSystem>().GetImage(AttachedImage, 150, 150);
            this.ImageView.Source = ImageSource.FromStream(() => new MemoryStream(bytes));

        }

        public double HeightMultiplier { get; set; }

        public ElementDescriptor RawDescriptor
        {
            get
            {
                return Descriptor;
            }
        }

        public string FieldValue
        {
            get
            {
                return this.AttachedImage == null ? "" : this.AttachedImage.ImageReference;
            }
            set
            {

            }
        }

        public string FieldNotShownValue
        {
            get
            {
                return "";
            }
        }

        public bool Tickable
        {
            get
            {
                return false;
            }
        }

        public bool Ticked
        {
            get
            {
                return false;
            }
        }

        public string FieldValValue
        {
            get
            {
                return null;
            }
        }

        public string PrepopValue
        {
            set
            {
                // not needed
            }
        }

        private bool isMandatory = false;
        public bool Mandatory
        {
            get
            {
                return this.isMandatory;
            }

            set
            {
                this.isMandatory = value;
                this.BackgroundColor = CoreAppTools.MandatoryRed;
            }
        }

        public FormProcessor FormProcessor { get; private set; }
    }
}
