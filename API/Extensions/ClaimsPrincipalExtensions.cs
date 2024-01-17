using System.Security.Claims;
using AutoMapper.Internal.Mappers;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id != null)
        {
            return int.Parse(id);
        }
        return null;
    }
}
