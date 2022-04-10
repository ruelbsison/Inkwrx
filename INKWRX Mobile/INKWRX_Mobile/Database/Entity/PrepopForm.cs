using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("PrepopForms")]
    public class PrepopForm : InkwrxBaseTable
    {
        public int Identifier { get; set; }
        public string Name { get; set; }
        public int Form { get; set; }
        public int VersionNumber { get; set; }
        public int User { get; set; }
        public int Status { get; set; }
    }
}
