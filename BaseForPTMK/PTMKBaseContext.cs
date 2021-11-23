using System.Data.Entity;

namespace BaseForPTMK
{
    public class PTMKBaseContext : DbContext
    {
        public PTMKBaseContext(): base("DbConnection") { }

        public DbSet<User> Users { get; set; }
    }
}
