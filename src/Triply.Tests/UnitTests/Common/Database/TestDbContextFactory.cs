using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Filtering;

namespace Triply.Tests.UnitTests.Common.Database;

/// <summary>Creates an isolated in-memory database per test.</summary>
public static class TestDbContextFactory
{
    public static TriplyDbContext Create(string? name = null)
    {
        var options = new DbContextOptionsBuilder<TriplyDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new TriplyDbContext(options);
    }

    public static ApplicationSieveProcessor CreateSieveProcessor()
        => new(Options.Create(new SieveOptions { DefaultPageSize = 10, MaxPageSize = 50, CaseSensitive = false }));
}
