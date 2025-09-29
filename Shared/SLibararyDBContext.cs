using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Shared
{
    public class SLibararyDBContext : DbContext
    {
        public SLibararyDBContext(DbContextOptions<SLibararyDBContext> options) : base(options)
        { }

        public DbSet<Book> Books { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
