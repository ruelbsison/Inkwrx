using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    public abstract class InkwrxBaseTable
    {
        public InkwrxBaseTable()
        {
            this.Id = -1;
        }

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
    }
}
