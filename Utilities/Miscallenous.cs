using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Discord;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Responses;

using Konscious.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace DiscordRepair.Api.Utilities;

internal static class Miscallenous
{
    // A static instance of the Random class, initialized with the default constructor.
    private static readonly Random random = new();

    // The salt used for Argon2 password hashing, loaded from the application resources and decoded from base64.
    private static readonly byte[] Argon2Salt = Convert.FromBase64String(Properties.Resources.Argon2Salt);

    /// <summary>
    /// Validates if a given username is valid or not.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <returns>True if the username is valid, false otherwise.</returns>
    internal static bool IsValidUsername(string username)
    {
        return !string.IsNullOrEmpty(username) && Regex.IsMatch(username, "^[-a-zA-Z0-9-()]+(\\s+[-a-zA-Z0-9-()]+)*$");
    }

    /// <summary>
    /// Generates a random Discord color.
    /// </summary>
    /// <returns>A Color instance representing a random color.</returns>
    internal static Color RandomDiscordColour() => new(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));

    /// <summary>
    /// Generates a random API token.
    /// </summary>
    /// <returns>A randomly generated API token.</returns>
    internal static string GenerateApiToken()
    {
        // Create a new instance of a cryptographically secure random number generator.
        using var cryptRNG = RandomNumberGenerator.Create();
        // Create a byte array to hold the token.
        byte[] tokenBuffer = new byte[64];
        // Fill the byte array with random data.
        cryptRNG.GetBytes(tokenBuffer);
        // Return the base64 encoded string representation of the token.
        return Convert.ToBase64String(tokenBuffer);
    }

    /// <summary>
    /// Gets the current user making the request.
    /// </summary>
    /// <returns>The current user.</returns>
    internal static async Task<Database.Models.User?> GetCurrentUserAsync(this HttpContext httpContext)
    {
        // Get the username of the current user from the HttpContext.
        var username = httpContext.WhoAmI();

        // Query the database for the user object with the matching username.
        // Return null if no user is found.
        return await new DatabaseContext().users.FirstOrDefaultAsync(x => x.username.Equals(username));
    }

    /// <summary>
    /// Verifies the given captcha code using the Google reCAPTCHA API.
    /// </summary>
    /// <param name="captchaCode">The captcha code to verify.</param>
    /// <returns>The captcha verification result.</returns>
    internal static async Task<ReCaptchaResponse?> VerifyCaptchaAsync(string captchaCode)
    {
        // Create a new HttpClient instance
        using var http = new HttpClient();

        // Create the form content with the captcha code and secret key
        var formContent = new Dictionary<string, string>
        {
            { "response", captchaCode },
            { "secret", Properties.Resources.ReCaptchaKey },
        };

        // Create a new form URL encoded content instance
        var content = new FormUrlEncodedContent(formContent);

        // Set the content type header to application/x-www-form-urlencoded
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

        // Send the POST request to the ReCaptcha API and await the response
        var requestResults = await http.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);

        // Deserialize the response JSON into a ReCaptchaResponse object
        return JsonConvert.DeserializeObject<ReCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Retrieves a user from the database by username or email asynchronously.
    /// </summary>
    /// <param name="identifier">The username or email of the user to retrieve.</param>
    /// <returns>The user object with the matching username or email, or null if not found.</returns>
    internal static async Task<Database.Models.User?> GetUserByUsernameOrEmailAsync(string identifier)
    {
        // Checks if the identifier parameter is null, empty or contains only whitespace characters.
        if (string.IsNullOrWhiteSpace(identifier))
        {
            // If the identifier is invalid, returns null.
            return null;
        }

        // Creates a new instance of the DatabaseContext class using the "await using" syntax.
        await using var database = new DatabaseContext();

        // Returns the first user object from the database that has a matching username or email to the given parameter, or null if not found.
        return await database.users.FirstOrDefaultAsync(x => x.username.ToLower().Equals(identifier.ToLower()) || x.email.ToLower().Equals(identifier.ToLower()));
    }

    /// <summary>
    /// Tries to extract the client IP address from the request headers or the connection properties.
    /// </summary>
    /// <param name="context">The HttpContext instance representing the current request.</param>
    /// <returns>The IP address of the client or null if it cannot be determined.</returns>
    internal static string? GetIPAddress(this HttpContext context)
    {
        // Try to extract the IP address from the X-Real-IP header first.
        if (context.Request.Headers.TryGetValue("X-Real-IP", out StringValues xRealIp))
        {
            return xRealIp;
        }
        // If X-Real-IP is not present, try X-Forwarded-For header.
        else
        {
            return context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedIp)
                ? (string?)xForwardedIp
                : (context.Connection.RemoteIpAddress?.ToString()); // If neither header is present, use the RemoteIpAddress property of the connection.
        }
    }

    /// <summary>
    /// Gets the username of the authenticated user from the HttpContext instance.
    /// </summary>
    /// <param name="httpContext">The HttpContext instance representing the current request.</param>
    /// <returns>The username of the authenticated user or an empty string if no user is authenticated.</returns>
    internal static string WhoAmI(this HttpContext httpContext) => httpContext.User.FindFirstValue("username") ?? "";

    /// <summary>
    /// Gets the API token of the authenticated user from the HttpContext instance.
    /// </summary>
    /// <param name="httpContext">The HttpContext instance representing the current request.</param>
    /// <returns>The API token of the authenticated user or an empty string if no user is authenticated.</returns>
    internal static string WhatIsMyToken(this HttpContext httpContext) => httpContext.User.FindFirstValue("accountToken") ?? "";

    /// <summary>
    /// Hashes the given password using the Argon2id algorithm.
    /// </summary>
    /// <param name="password">The password to be hashed.</param>
    /// <returns>A string representing the hashed password.</returns>
    internal static async ValueTask<string> HashPasswordAsync(string password)
    {
        // Create an instance of Argon2id with the given password
        // and set the salt, degree of parallelism, iterations, and memory size.
        using Argon2id argon2 = new(Convert.FromBase64String(password))
        {
            Salt = Argon2Salt,
            DegreeOfParallelism = 2,
            Iterations = 4,
            MemorySize = 128 * 128
        };
        // Generate a byte array representing the hashed password using Argon2id.
        byte[] hashed = await argon2.GetBytesAsync(128);

        // Convert the hashed byte array to a base64-encoded string and return it.
        return Convert.ToBase64String(hashed);

    }

    /// <summary>
    /// Verifies whether the given password matches the given hashed password.
    /// </summary>
    /// <param name="password">The password to be verified.</param>
    /// <param name="hashedPassword">The hashed password to be compared with.</param>
    /// <returns>True if the password matches the hashed password; otherwise, false.</returns>
    internal static async ValueTask<bool> VerifyHashAsync(string password, string hashedPassword) => await HashPasswordAsync(password) == hashedPassword;
    //internal static async ValueTask<string> HashPasswordAsync(string password)
    //{
    //    using Argon2id argon2 = new(Convert.FromBase64String(password))
    //    {
    //        Salt = Convert.FromBase64String(Properties.Resources.Argon2Salt),
    //        DegreeOfParallelism = 2, // four cores
    //        Iterations = 4,
    //        MemorySize = 128 * 128 // .5 GB
    //    };
    //    byte[]? hashed = await argon2.GetBytesAsync(128);
    //    return Convert.ToBase64String(hashed);
    //}
    //internal static async ValueTask<bool> VerifyHashAsync(string password, string hashedPassword) => await HashPasswordAsync(password) == hashedPassword;
}
//internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));

//internal static string GenerateApiToken()
//{
//    using var cryptRNG = RandomNumberGenerator.Create();
//    byte[] tokenBuffer = new byte[64];
//    cryptRNG.GetBytes(tokenBuffer);
//    return Convert.ToBase64String(tokenBuffer);
//}

//internal static string? GetIPAddress(this HttpContext context)
//{
//    return context.Request.Headers.TryGetValue("X-Real-IP", out StringValues xRealIp)
//        ? xRealIp
//        : context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedIp)
//        ? xForwardedIp
//        : context.Connection.RemoteIpAddress?.ToString();
//}

//internal static string WhoAmI(this HttpContext httpContext)
//{
//    return httpContext.User.Claims.First(x => x.Type == "username").Value;
//}

//internal static string WhatIsMyToken(this HttpContext httpContext)
//{
//    return httpContext.User.Claims.First(x => x.Type == "accountToken").Value;
//}

//internal static async ValueTask<string> HashPasswordAsync(string password)
//{
//    using Argon2id argon2 = new(Convert.FromBase64String(password))
//    {
//        Salt = Convert.FromBase64String(Properties.Resources.Argon2Salt),
//        DegreeOfParallelism = 2, // four cores
//        Iterations = 4,
//        MemorySize = 128 * 128 // .5 GB
//    };
//    byte[]? hashed = await argon2.GetBytesAsync(128);
//    return Convert.ToBase64String(hashed);
//}
//internal static async ValueTask<bool> VerifyHashAsync(string password, string hashedPassword) => await HashPasswordAsync(password) == hashedPassword;

