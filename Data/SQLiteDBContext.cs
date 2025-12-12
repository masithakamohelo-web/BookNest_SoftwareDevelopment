using ASPNETCore_DB.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_DB.Data
{
    public class SQLiteDBContext : DbContext
    {
        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
    
     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Consumer>().ToTable("Consumer");
            
           


        }
    }
}