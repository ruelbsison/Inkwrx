using INKWRX_Mobile.Database;
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
    public class HistoryItemModel : BaseModel
    {
        public HistoryItemModel(Transaction trans)
        {
            this.Transaction = trans;
        }

        private Transaction transaction = null;

        public Transaction Transaction
        {
            get
            {
                return this.transaction;
            }
            set
            {
                SetField(ref this.transaction, value);
            }
        }

        public string FormName
        {
            get
            {
                return this.Transaction.FormName;
            }
        }

        public string SavedLabel
        {
            get
            {
                return this.Transaction.SavedDate == null
                    ? "<Form Not Parked>"
                    : string.Format("{0} - Saved", this.Transaction.SavedDate.Value.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }

        public string StartedLabel
        {
            get
            {
                return string.Format("{0} - Started", this.Transaction.StartedDate.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }

        public string SentLabel
        {
            get
            {
                return this.Transaction.SentDate == null
                    ? "<Form Not Sent>"
                    : string.Format("{0} - Sent", this.Transaction.SentDate.Value.ToString("dd/MM/yyyy HH:mm:ss"));
            }
        }

        public bool ArrowIsVisible
        {
            get
            {
                return this.Transaction.Status == (int)DatabaseHelper.Status.Autosaved
                    || this.Transaction.Status == (int)DatabaseHelper.Status.Parked
                    || this.Transaction.Status == (int)DatabaseHelper.Status.Sent;
            }
        }

        public ImageSource IconSource
        {
            get
            {
                switch (this.Transaction.Status)
                {
                    case (int)DatabaseHelper.Status.Sent:
                        return CoreAppTools.GetImageSource("Icons/HistoryScreen/iw_app_ios_icon_send.png");
                    case (int)DatabaseHelper.Status.Pending:
                        return CoreAppTools.GetImageSource("Icons/HistoryScreen/iw_app_ios_icon_pending.png");
                    case (int)DatabaseHelper.Status.Parked:
                        return CoreAppTools.GetImageSource("Icons/HistoryScreen/iw_app_ios_icon_parked.png");
                    case (int)DatabaseHelper.Status.Autosaved:
                        return CoreAppTools.GetImageSource("Icons/HistoryScreen/iw_app_ios_icon_autosave.png");
                    default:
                        return CoreAppTools.GetImageSource("Icons/HistoryScreen/iw_app_ios_icon_history.png");
                }
            }
        }
    }
}
