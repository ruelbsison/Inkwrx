using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Database.Entity
{
    [Table("Users")]
    public class User : InkwrxBaseTable
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
