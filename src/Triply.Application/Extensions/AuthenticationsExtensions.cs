using Triply.Application.DTOs.Authentications;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for authentications.</summary>
public static class AuthenticationsExtensions
{
    /// <summary>Maps the authentications to a Triply user.</summary>
    public static TriplyUser ToTriplyUser(this RegisterUserRequest request)
        => new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Username,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
        };

    /// <summary>Maps the authentications to a login API response.</summary>
    public static LoginApiResponse ToLoginApiResponse(this LoginResponse response)
        => new(response.Name, response.Access);
}