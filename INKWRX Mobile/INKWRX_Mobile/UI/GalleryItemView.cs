using INKWRX_Mobile.Views.PageModels;
using INKWRXPhotoTools_Mobile;
using System;
using System.IO;

using Xamarin.Forms;
using static INKWRXPhotoTools_Mobile.PhotoTools;
using System.Threading.Tasks;

namespace INKWRX_Mobile.UI
{
    public class GalleryItemView : ContentView
    {
        public GalleryItemView()
        {
            var rel = new RelativeLayout();
            rel.BackgroundColor = Color.White;
            rel.HeightRequest = 170;
            rel.WidthRequest = 170;
            ImageDisplay = new Image();
            ImageDisplay.BackgroundColor = Color.White;
            ImageDisplay.Aspect = Aspect.AspectFit;
            ImageDisplay.HorizontalOptions = LayoutOptions.FillAndExpand;
            ImageDisplay.VerticalOptions = LayoutOptions.FillAndExpand;
            notAttached.SetBinding(Image.SourceProperty, "AttachIconSource");
            attached.SetBinding(VisualElement.OpacityProperty, "Attached", converter: new HomeBoolToOpacityValueConverter());
            rel.Children.Add(this.ImageDisplay, Constraint.Constant(10), Constraint.Constant(10), 
                Constraint.RelativeToParent((parent) => { return parent.Width - 20; }), Constraint.RelativeToParent((parent) => { return parent.Height - 20; }));
            
            rel.Children.Add(this.notAttached, Constraint.RelativeToParent((parent) => { return parent.Width - 35; }), Constraint.RelativeToParent((parent) => { return parent.Height - 35; }),
                            Constraint.Constant(25), Constraint.Constant(25));
            rel.Children.Add(this.attached, Constraint.RelativeToParent((parent) => { return parent.Width - 35; }), Constraint.RelativeToParent((parent) => { return parent.Height - 35; }),
                Constraint.Constant(25), Constraint.Constant(25));
            Content = rel;
            
        }

        private Image notAttached = new Image
        {
            BackgroundColor = Color.White,
            WidthRequest = 25,
            HeightRequest = 25,
            Aspect = Aspect.AspectFit,
            Opacity = 0.5
        };
        private Image attached = new Image {
            BackgroundColor = Color.White,
            WidthRequest = 25,
            HeightRequest = 25,
            Aspect = Aspect.AspectFit,
            Source = CoreAppTools.GetImageSource("bar_icon_attach_active.png")
        };
        private Image ImageDisplay = null;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var model = (GalleryItemModel)this.BindingContext;
            this.ImageDisplay.Source = model.ImageEntrySourceCached;
        }
    }
}
