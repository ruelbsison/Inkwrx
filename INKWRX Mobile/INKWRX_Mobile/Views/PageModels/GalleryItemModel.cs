using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using INKWRXPhotoTools_Mobile;
using Xamvvm;
using Xamarin.Forms;
using System.IO;

namespace INKWRX_Mobile.Views.PageModels
{
    public class GalleryItemModel : BaseModel
    {
        public GalleryItemModel(ImageEntry entry)
        {
            this.ImageEntry = entry;
            this.SetImageEntrySource();
        }

        private async void SetImageEntrySource()
        {
            byte[] imgData = await DependencyService.Get<PhotoTools.IPhotoFileSystem>().GetImage(this.ImageEntry, 150, 150);
            this.ImageEntrySourceCached = ImageSource.FromStream(() => new MemoryStream(imgData));
        }

        public ImageEntry ImageEntry { get; set; }

        bool attached = false;
        
        public bool Attached
        {
            get
            {
                return attached;
            }
            set
            {
                SetField(ref attached, value);
            }
        }

        public ImageSource AttachIconSource
        {
            get
            {
                return CoreAppTools.GetImageSource(this.ImageEntry.ImageType == ImageEntry.ImageEntryType.Gallery
                    ? "bar_icon_gallery.png"
                    : "bar_icon_camera.png");
            }
        }

        public string ImageReference { get { return this.ImageEntry.ImageReference; } }
        public DateTime CreatedDate { get { return this.ImageEntry.CreatedDate; } }
        public ImageEntry.ImageEntryType ImageType { get { return this.ImageEntry.ImageType; } }
        public ImageSource ImageEntrySourceCached { get; set; }
    }
}
