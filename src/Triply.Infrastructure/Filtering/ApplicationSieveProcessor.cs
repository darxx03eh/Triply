using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Filtering;

public class ApplicationSieveProcessor(IOptions<SieveOptions> options) : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<City>(c => c.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<City>(c => c.Country)
            .CanFilter()
            .CanSort();

        mapper.Property<City>(c => c.CreatedAt)
            .CanSort();

        return mapper;
    }
}