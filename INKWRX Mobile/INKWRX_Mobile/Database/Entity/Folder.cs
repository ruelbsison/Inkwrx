using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Folders")]
    public class Folder : InkwrxBaseTable
    {
        public Folder() : base()
        {
            this.Parent = -1;
        }
        public string Name { get; set; }
        public int User { get; set; }
        public int Parent { get; set; }
    }
}
