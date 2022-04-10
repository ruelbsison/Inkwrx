using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Fields")]
    public class Field : InkwrxBaseTable
    {
        public string Name { get; set; }
        public string NotShownValue { get; set; }
        public string ShownValue { get; set; }
        public bool Tickable { get; set; }
        public bool Ticked { get; set; }
        public int Transaction { get; set; }

        #region AutoValues

        [Ignore]
        public DateTime Date {
            get
            {
                return Convert.ToDateTime(this.ShownValue);
            }
            set
            {
                this.ShownValue = value.ToString("yyyy/MM/dd");
            }
        }

        [Ignore]
        public DateTime Time
        {
            get
            {
                return Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy ") + this.ShownValue);
            }
            set
            {
                this.ShownValue = value.ToString("HH:mm:ss.ffff");
            }
        }

        #endregion
    }
}
