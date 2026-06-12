using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public partial class QueryContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ensure that the connection string is only set if it hasn't been configured yet,
        // this allows using different connection strings for testing.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=../ViaEventAssociation.Infrastructure.EfcDmPersistence/database.sqlite");
        }
    }
}
