namespace Triply.Application.Interfaces.Services;

/// <summary>Gives access to the i current user of the request.</summary>
public interface ICurrentUserAccessor
{
    /// <summary>Gets the identifier of the user.</summary>
    Guid UserId { get; }
    /// <summary>Gets whether the current user accessor is admin.</summary>
    bool IsAdmin { get; }
}