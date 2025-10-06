using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SLibrary.DataAccess.Models;



namespace SLibrary.DataAccess
{
    public class SLibararyDBContext : IdentityDbContext<AppUser>
    {
        public SLibararyDBContext(DbContextOptions<SLibararyDBContext> options) : base(options)
        { }

        public DbSet<Book> Books { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
