using Amiasea.Enterprise.Core.Data.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Enterprise.Test.Core.Api;

public sealed class HandlersFixture : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public EnterpriseDbContext Db { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");

        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<EnterpriseDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new EnterpriseDbContext(options);

        await Db.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Db.DisposeAsync();
        await _connection.DisposeAsync();
    }
}