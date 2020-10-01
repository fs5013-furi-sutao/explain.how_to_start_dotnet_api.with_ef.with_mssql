using Microsoft.EntityFrameworkCore;
using studentapi.Models;

namespace studentapi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Student> Students { get; set; }
    }
}