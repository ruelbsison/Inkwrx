using DLToolkit.Forms.Controls;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Util;
using INKWRX_Mobile.Views.PageModels;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static INKWRXPhotoTools_Mobile.PhotoTools;

namespace INKWRX_Mobile.Views
{
    public class GalleryAttachPage : InkwrxBasePage
    {
        public GalleryAttachPage(InkwrxBasePage parent, FormProcessor processor) : this(parent, null, processor)
        {

        }

        public GalleryAttachPage(InkwrxBasePage parent, TabletImageView tiv, FormProcessor processor) : base("Attach Image", "Backgrounds/FormScreen/iw_app_ios_background_form.png", parent)
        {
            this.tabletImageView = tiv;
            this.FormProcessor = processor;
            var stack = new StackLayout();
            this.HeaderLabel.Text = tiv == null ? string.Format("Attach Image ({0})", this.FormProcessor.AttachedImages.Count) : "Embed Image";

            this.galleryFlow = new FlowListView();

            this.galleryFlow.BackgroundColor = CoreAppTools.LightSilver;
            this.galleryFlow.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.galleryFlow.VerticalOptions = LayoutOptions.FillAndExpand;
            this.galleryFlow.FlowColumnTemplate = new DataTemplate(typeof(GalleryItemView));
            this.galleryFlow.FlowColumnMinWidth = 170;
            this.galleryFlow.HasUnevenRows = true;
            this.GalleryItems = new ObservableCollection<GalleryItemModel>();
            this.galleryFlow.FlowItemsSource = this.GalleryItems;
            this.galleryFlow.FlowItemTapped += (sender, eventArgs) =>
            {
                var item = (GalleryItemModel)eventArgs.Item;

                if (tiv != null)
                {
                    
                    item.Attached = true;
                    tiv.AttachedImage = item.ImageEntry;
                    
                    if (!this.FormProcessor.AttachedImages.Any(x => x.ImageReference == item.ImageReference))
                    {
                        this.FormProcessor.AttachedImages.Add(item.ImageEntry);
                    }
                    App.Current.MainPage = this.ParentPage;
                }
                else
                {
                    item.Attached = !item.Attached;
                    if (item.Attached)
                    {
                        //intentionally nested - we don't want the previous if's else to include the following logic...
                        if (!this.FormProcessor.AttachedImages.Any(x => x.ImageReference == item.ImageReference))
                        {
                            this.FormProcessor.AttachedImages.Add(item.ImageEntry);
                        }
                    }
                    else if (this.FormProcessor.AttachedImages.Any(x => x.ImageReference == item.ImageReference))
                    {
                        var remove = this.FormProcessor.AttachedImages.FirstOrDefault(x => x.ImageReference == item.ImageReference);
                        if (remove != null)
                        {
                            this.FormProcessor.AttachedImages.Remove(remove);
                            var embedded = this.FormProcessor.AllFields.OfType<TabletImageView>().Where(x => x.AttachedImage != null && x.AttachedImage.ImageReference == remove.ImageReference)
                                    .ToList();
                            foreach (var embed in embedded)
                            {
                                embed.AttachedImage = null;
                            }
                        }
                    }
                    this.HeaderLabel.Text = string.Format("Attach Image ({0})", this.FormProcessor.AttachedImages.Count);
                }

                this.galleryFlow.SelectedItem = null;
            };

            this.galleryFlow.FlowTappedBackgroundColor = Color.Transparent;
            
            stack.Children.Add(this.galleryFlow);
            this.PageContent.Content = stack;

            this.refreshImages();
        }

        public void ScrollListTo(object item)
        {
            galleryFlow.ScrollTo(item, ScrollToPosition.MakeVisible, true);
        }
        

        private FlowListView galleryFlow;

        public ObservableCollection<GalleryItemModel> GalleryItems { get; set; }
        public int ScrollY { get; private set; }
        public FormProcessor FormProcessor { get; private set; }

        private void refreshImages()
        {
            Task.Run(async () =>
            {
                var formView = (FormViewPage)this.ParentPage;
                var form = formView.Processor.CurrentTransaction;


                var gallItems = await DependencyService.Get<IPhotoFileSystem>().GetGalleryImages();
                var cameraItems = await DependencyService.Get<IPhotoFileSystem>().GetCameraImages(form == null ? "-1" : form.Id.ToString());

                var obs = cameraItems.Select(x => new GalleryItemModel(x)
                { Attached = this.FormProcessor.AttachedImages.Any(img => img.ImageReference == x.ImageReference) })
                             .OrderByDescending(x => x.CreatedDate).ToList();
                obs.AddRange(gallItems.Select(x => new GalleryItemModel(x)
                { Attached = this.FormProcessor.AttachedImages.Any(img => img.ImageReference == x.ImageReference) })
                             .OrderByDescending(x => x.CreatedDate).ToList());

                this.GalleryItems = new ObservableCollection<GalleryItemModel>(obs);
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.galleryFlow.FlowItemsSource = this.GalleryItems;
                });
            });
        }

        private TabletImageView tabletImageView = null;
    }
}
