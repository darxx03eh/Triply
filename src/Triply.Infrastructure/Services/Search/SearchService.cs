using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Search;

/// <summary>Implements the search operations.</summary>
public partial class SearchService(
    ISearchRepository searchRepository,
    ILogger<SearchService> logger) : ISearchService
{
    
}
