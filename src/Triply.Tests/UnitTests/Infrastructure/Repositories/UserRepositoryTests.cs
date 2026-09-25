using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(_db);
        var user = TestData.User("Mahmoud.D", "Mahmoud@Triply.com");
        user.NormalizedUserName = user.UserName!.ToUpperInvariant();
        user.NormalizedEmail = user.Email!.ToUpperInvariant();
        var deleted = TestData.User("ghost", "ghost@triply.com");
        deleted.NormalizedUserName = "GHOST";
        deleted.NormalizedEmail = "GHOST@TRIPLY.COM";
        deleted.PhoneNumber = "+970599999999";
        deleted.IsDeleted = true;
        _db.Users.AddRange(user, deleted);
        _db.SaveChanges();
    }

    [Theory]
    [InlineData("mahmoud@triply.com", true)]
    [InlineData("MAHMOUD@TRIPLY.COM", true)]
    [InlineData("other@triply.com", false)]
    [InlineData("ghost@triply.com", false)]
    public async Task IsEmailExistsAsync_UsesNormalizedEmail(string email, bool expected)
    {
        Assert.Equal(expected, await _repository.IsEmailExistsAsync(email));
    }

    [Theory]
    [InlineData("mahmoud.d", true)]
    [InlineData("Mahmoud.D", true)]
    [InlineData("someone", false)]
    public async Task IsUsernameExistsAsync_UsesNormalizedName(string username, bool expected)
    {
        Assert.Equal(expected, await _repository.IsUsernameExistsAsync(username));
    }

    [Fact]
    public async Task GetByPhoneNumberAsync_FindsActiveUserOnly()
    {
        Assert.Equal("Mahmoud.D", (await _repository.GetByPhoneNumberAsync("+970591234567"))!.UserName);
        Assert.Null(await _repository.GetByPhoneNumberAsync("+970599999999"));
        Assert.Null(await _repository.GetByPhoneNumberAsync("+970500000000"));
    }

    public void Dispose() => _db.Dispose();
}
