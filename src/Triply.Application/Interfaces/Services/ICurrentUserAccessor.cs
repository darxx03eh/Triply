namespace Triply.Application.Interfaces.Services;

public interface ICurrentUserAccessor
{
    int UserId { get; }
    bool IsAdmin { get; }
}