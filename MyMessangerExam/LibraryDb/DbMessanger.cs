using LibraryMessage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDb
{
    public class DbMessanger:DbContext
    {
        public DbMessanger(string connection) : base(connection) { }
        public DbMessanger(){ }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<MyMessage> Messages { get; set; }       

    }
}
