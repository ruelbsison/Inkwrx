using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("StrokePoints")]
    public class StrokePoint : InkwrxBaseTable
    {
        public int Path { get; set; }

        [Column("XValue")]
        public double X { get; set; }

        [Column("YValue")]
        public double Y { get; set; }
    }
}
