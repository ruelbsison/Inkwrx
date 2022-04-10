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
    public class HomePageLinkModel : BaseModel
    {
        public enum PageType
        {
            Home,
            HistoryAll,
            HistoryParked,
            HistoryPending,
            HistorySent,
            HistoryAutosaved,
            Prepop,
            Forms,
            FormView
        }

        public HomePageLinkModel (PageType pageType, string label, string iconPath)
        {
            this.Page = pageType;
            this.IconSource = CoreAppTools.GetImageSource(iconPath);
            this.Label = label;
        }

        private PageType pageType = PageType.Home;
        private bool canView = false;
        private ImageSource iconSource = null;
        private int indicator = 0;
        private string label = null;

        public string Label
        {
            get { return this.label; }
            set { SetField(ref this.label, value); }
        }

        public bool CanView
        {
            get { return this.canView; }
            private set { SetField(ref this.canView, value); }
        }

        public PageType Page
        {
            get { return this.pageType; }
            set { SetField(ref this.pageType, value); }
        }

        public ImageSource IconSource
        {
            get { return iconSource; }
            set { SetField(ref iconSource, value); }
        }
                
        public int Indicator
        {
            get { return this.indicator; }
            set
            {
                SetField(ref this.indicator, value);
                this.CanView = this.Indicator > 0;
            }
        }
    }
}
