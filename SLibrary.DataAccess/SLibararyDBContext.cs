using Microsoft.EntityFrameworkCore;
using SLibrary.DataAccess.Models;



namespace SLibrary.DataAccess
{
    public class SLibararyDBContext : DbContext
    {
        public SLibararyDBContext(DbContextOptions<SLibararyDBContext> options) : base(options)
        { }

        public DbSet<Book> Books { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
