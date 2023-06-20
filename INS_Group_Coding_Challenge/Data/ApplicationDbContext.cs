using Microsoft.EntityFrameworkCore;
using INS_Group_Coding_Challenge.Models;
namespace INS_Group_Coding_Challenge.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Note { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User()
                { 
                    Id = 1,
                    UserName = "Andy Zhu",
                }
             );
           
        }
    }
}
