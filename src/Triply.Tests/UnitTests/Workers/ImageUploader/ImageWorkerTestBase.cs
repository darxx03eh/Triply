using ImageUploader.IProviders;
using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Triply.Application.Interfaces.Repositories;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Database;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.XUnitTests.Workers.ImageUploader;

/// <summary>
/// Runs the worker's handlers against an in-memory database: the consumer mock captures the handlers
/// the hosted service subscribes with, so tests can deliver messages directly.
/// </summary>
public abstract class ImageWorkerTestBase : IDisposable
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    protected readonly Mock<IMessageConsumer> Consumer = new();
    protected readonly Mock<ICloudinaryUploader> Cloudinary = new();
    protected readonly ServiceProvider Services;
    protected readonly TempStorage Storage = new();

    protected ImageWorkerTestBase()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => TestDbContextFactory.Create(_databaseName));
        services.AddScoped<IHotelImageRepository>(sp => new HotelImageRepository(
            sp.GetRequiredService<TriplyDbContext>()));
        services.AddScoped<IRoomImageRepository>(sp => new RoomImageRepository(
            sp.GetRequiredService<TriplyDbContext>()));
        services.AddScoped<ICityRepository>(sp =>
            new CityRepository(sp.GetRequiredService<TriplyDbContext>(), 
                TestDbContextFactory.CreateSieveProcessor()));
        Services = services.BuildServiceProvider();
        Directory.CreateDirectory(Storage.Path);
    }

    /// <summary>A fresh context on the same in-memory database, to seed or assert.</summary>
    protected TriplyDbContext Db() => TestDbContextFactory.Create(_databaseName);

    protected string StageFile(string name = "image.png")
    {
        var path = Path.Combine(Storage.Path, name);
        File.WriteAllBytes(path, TestImages.Png);
        return path;
    }

    public void Dispose()
    {
        Services.Dispose();
        Storage.Dispose();
    }
}
