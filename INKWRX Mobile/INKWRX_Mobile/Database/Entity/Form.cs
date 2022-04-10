using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Forms")]
    public class Form : InkwrxBaseTable
    {
        public Form() : base()
        {
            this.ParentFolder = -1;
        }

        public int Status { get; set; }

        public string FormIdentifier { get; set; }
        public string FormName { get; set; }
        public int User { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int ParentFolder { get; set; }
    }
}
