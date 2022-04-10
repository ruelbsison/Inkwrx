using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("StrokePaths")]
    public class StrokePath : InkwrxBaseTable
    {
        public string FieldName { get; set; }
        public int Transaction { get; set; }
        public double FieldY { get; set; }
        public double FieldX { get; set; }
    }
}
