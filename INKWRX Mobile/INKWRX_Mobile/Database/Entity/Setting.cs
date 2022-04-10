using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Settings")]
    public class Setting : InkwrxBaseTable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int User { get; set; }
    }
}
