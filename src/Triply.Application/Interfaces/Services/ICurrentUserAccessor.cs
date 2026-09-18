namespace Triply.Application.Interfaces.Services;

public interface ICurrentUserAccessor
{
    Guid UserId { get; }
    bool IsAdmin { get; }
}