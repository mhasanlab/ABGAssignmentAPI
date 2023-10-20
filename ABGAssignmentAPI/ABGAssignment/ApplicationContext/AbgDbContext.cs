using ABGAssignment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ABGAssignment.ApplicationContext
{
    public class AbgDbContext : DbContext
    {
        public AbgDbContext(DbContextOptions<AbgDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

    }
        
}
