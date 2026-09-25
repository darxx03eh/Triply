using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedSearchServiceTests
{
    private readonly Mock<ISearchService> _inner = new();

    [Fact]
    public async Task SearchHotelsAsync_Valid_CallsInner()
    {
        var request = new SearchHotelsRequest();

        await new ValidatedSearchService(_inner.Object, FakeValidators.Passing<SearchHotelsRequest>())
            .SearchHotelsAsync(request);

        _inner.Verify(s => s.SearchHotelsAsync(request, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task SearchHotelsAsync_Invalid_ThrowsAndSkipsInner()
    {
        var service = new ValidatedSearchService(_inner.Object, 
            FakeValidators.Failing<SearchHotelsRequest>("CheckIn", "past"));

        var exception = await Assert.ThrowsAsync<UnprocessableEntityException>(() => 
            service.SearchHotelsAsync(new SearchHotelsRequest()));

        Assert.Equal("past", exception.Errors["CheckIn"].Single());
        _inner.VerifyNoOtherCalls();
    }
}
