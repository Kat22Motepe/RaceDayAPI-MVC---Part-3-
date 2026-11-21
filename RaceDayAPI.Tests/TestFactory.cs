using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.Infrastructure;
using System.Data.Common;

namespace RaceDay.API.Tests;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private DbConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remove the real SQL Server database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RaceDayDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // 2. Create an open SQLite connection to keep the database alive in memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 3. Use SQLite for the tests
            services.AddDbContext<RaceDayDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}