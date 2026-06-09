using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public class DmContextFactory : IDesignTimeDbContextFactory<DmContext>
{
    public DmContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DmContext>();

        optionsBuilder.UseSqlite("Data Source=database.sqlite");

        return new DmContext(optionsBuilder.Options);
    }
}
