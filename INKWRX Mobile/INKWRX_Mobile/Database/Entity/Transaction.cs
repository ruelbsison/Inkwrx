using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Transactions")]
    public class Transaction : InkwrxBaseTable
    {
        public Transaction() : base ()
        {
            this.AutosavedParent = -1;
            this.PrepopId = -1;
        }

        public DateTime StartedDate { get; set; }
        public DateTime? SavedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? AutosavedDateTime { get; set; }
        public int AutosavedParent { get; set; }
        public int Form { get; set; }
        public int PrepopId { get; set; }
        public DateTime OriginalAddedDate { get; set; }
        public int Status { get; set; }
        public int User { get; set; }
        
        /// <summary>
        /// Form Name - Used in case Form is deleted before sending
        /// </summary>
        public string FormName { get; set; }
    }
}
