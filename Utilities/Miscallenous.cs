using System.Security.Cryptography;

using Discord;

using Konscious.Security.Cryptography;

using Microsoft.Extensions.Primitives;

namespace DiscordRepair.Api.Utilities;

internal static class Miscallenous
{
    internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));

    internal static string GenerateApiToken()
    {
        using var cryptRNG = RandomNumberGenerator.Create();
        byte[] tokenBuffer = new byte[64];
        cryptRNG.GetBytes(tokenBuffer);
        return Convert.ToBase64String(tokenBuffer);
    }

    internal static string? GetIPAddress(this HttpContext context)
    {
        return context.Request.Headers.TryGetValue("X-Real-IP", out StringValues xRealIp)
            ? xRealIp
            : context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedIp)
            ? xForwardedIp
            : context.Connection.RemoteIpAddress?.ToString();
    }

    internal static string WhoAmI(this HttpContext httpContext)
    {
        return httpContext.User.Claims.First(x => x.Type == "username").Value;
    }

    internal static string WhatIsMyToken(this HttpContext httpContext)
    {
        return httpContext.User.Claims.First(x => x.Type == "accountToken").Value;
    }

    internal static async ValueTask<string> HashPasswordAsync(string password)
    {
        using Argon2id argon2 = new(Convert.FromBase64String(password))
        {
            Salt = Convert.FromBase64String(Properties.Resources.Argon2Salt),
            DegreeOfParallelism = 2, // four cores
            Iterations = 4,
            MemorySize = 128 * 128 // .5 GB
        };
        byte[]? hashed = await argon2.GetBytesAsync(128);
        return Convert.ToBase64String(hashed);
    }
    internal static async ValueTask<bool> VerifyHash(string password, string hashedPassword) => await HashPasswordAsync(password) == hashedPassword;
}
