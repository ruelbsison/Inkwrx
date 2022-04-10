using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("PrepopFields")]
    public class PrepopField : InkwrxBaseTable
    {
        public int PrepopForm { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
