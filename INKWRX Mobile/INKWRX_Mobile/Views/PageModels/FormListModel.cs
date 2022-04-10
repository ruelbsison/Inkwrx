using INKWRX_Mobile.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamvvm;

namespace INKWRX_Mobile.Views.PageModels
{
    public class FormListModel : BaseModel, INotifyPropertyChanged
    {
        public FormListModel (Form form)
        {
            this.FormItem = form;
        }

        public FormListModel(Folder folder)
        {
            this.FolderItem = folder;
        }

        public string ItemName
        {
            get
            {
                return this.FormItem == null ? this.FolderItem.Name : this.FormItem.FormName;
            }
        }

        private Folder folderItem = null;
        private Form formItem = null;
        private bool selected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private Color backgroundColor;

        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                SetField(ref this.selected, value);
                this.BackgroundColor = value ? Color.Silver : Color.White;
            }
        }

        public ImageSource IconSource
        {
            get
            {
                return CoreAppTools.GetImageSource(this.FormItem == null
                    ? "Icons/FormScreen/iw_app_ios_icon_folder.png"
                    : "Icons/FormScreen/iw_app_ios_icon_form.png");
            }
        }

        public Folder FolderItem {
            get
            {
                return this.folderItem;
            }
            set
            {
                SetField(ref this.folderItem, value);
            }
        }
        public Form FormItem {
            get
            {
                return this.formItem;
            }
            set
            {
                SetField(ref this.formItem, value);
            }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
                }
            }
        }
    }
}
