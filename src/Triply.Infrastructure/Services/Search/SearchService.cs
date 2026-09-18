using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Search;

public partial class SearchService(ISearchRepository searchRepository) : ISearchService
{
    
}
