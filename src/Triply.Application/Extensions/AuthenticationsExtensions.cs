using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Extensions;

public static class AuthenticationsExtensions
{
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
}