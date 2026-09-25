using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Options;
using Triply.Infrastructure.Services.Cities;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

/// <summary>Shared mocks for the CityService tests; every test class gets a fresh instance.</summary>
public abstract class CityServiceTestBase : IDisposable
{
    protected readonly Mock<ICityRepository> CityRepository = new();
    protected readonly Mock<IMessagePublisher> Publisher = new();
    protected readonly TempStorage Storage = new();
    protected readonly CityService Service;

    protected CityServiceTestBase()
    {
        CityRepository.Setup(r => r.GetHotelsCountAsync(
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());
        Service = new CityService(
            CityRepository.Object,
            Options.Create(new ImageUploadOptions { SharedStoragePath = Storage.Path }),
            Publisher.Object,
            NullLogger<CityService>.Instance);
    }

    public void Dispose() => Storage.Dispose();
}
