using Microsoft.EntityFrameworkCore;
using SixtyNames.Entities;

namespace SixtyNames.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LegalEntity> LegalEntities { get; set; }
        public DbSet<PhysicalPerson> PhysicalPersons { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
