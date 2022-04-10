using INKWRX_Mobile.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamvvm;

namespace INKWRX_Mobile.Views.PageModels
{
    public class PrepopItemModel : BaseModel
    {
        public PrepopItemModel (PrepopForm form)
        {
            this.PrepopForm = form;
            this.PrepopName = form.Name;
            this.setFormName();
        }

        private async void setFormName ()
        {
            var form = await App.DatabaseHelper.GetFormAsync(this.PrepopForm.Form);
            this.FormName = form == null ? "<Unavailable Form>" : form.FormName;
        }

        private string prepopName = "";
        private string formName = "";

        public string PrepopName
        {
            get
            {
                return this.prepopName;
            }
            set
            {
                SetField(ref this.prepopName, value);
            }
        }

        public string FormName
        {
            get
            {
                return this.formName;
            }
            set
            {
                SetField(ref this.formName, value);
            }
        }

        public PrepopForm PrepopForm { get; set; }
        
    }
}
