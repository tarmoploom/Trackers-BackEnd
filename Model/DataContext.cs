using Microsoft.EntityFrameworkCore;

namespace TrackerApplication.Model {
    public class DataContext : DbContext {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasData(
                Data.User1, // hidden from git
                Data.User2  // hidden from git
            );
        }
    }
}
