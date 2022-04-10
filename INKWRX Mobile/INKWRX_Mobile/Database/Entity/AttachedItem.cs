using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("AttachedItems")]
    public class AttachedItem : InkwrxBaseTable
    {
        public int ItemSource { get; set; }
        public int ItemType { get; set; }
        public int Transaction { get; set; }
        public string Reference { get; set; }
        public int Status { get; set; }
        public DateTime? SentDateTime { get; set; }
    }
}
