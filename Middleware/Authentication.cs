using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DiscordRepair.Api.Middleware;

/// <summary>
/// The Authentication class for handling API token authentication.
/// </summary>
public class Authentication
{
    /// <summary>
    /// The internal Schemes class for storing authentication scheme names.
    /// </summary>
    internal static class Schemes
    {
        /// <summary>
        /// The name of the main authentication scheme.
        /// </summary>
        internal const string MainScheme = "MainScheme";
    }

    /// <summary>
    /// The authentication handler for API token authentication.
    /// </summary>
    public class Handler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Services.TokenLoader _tokenLoader;

        /// <summary>
        /// Constructor for the Handler class.
        /// </summary>
        public Handler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, Services.TokenLoader tokenLoader) : base(options, logger, encoder, clock) => _tokenLoader = tokenLoader;

        /// <summary>
        /// Method for handling authentication for an incoming API token.
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the request contains an Authorization header
            if (Request.Headers.ContainsKey("Authorization") is false)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            // Get the value of the Authorization header
            string? token = Request.Headers["Authorization"].ToString();

            // Check if the token is null or empty
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Token"));
            }

            // Remove any "Authorization " prefix from the token value
            if (token.Contains("Authorization "))
            {
                token = token.Replace("Authorization ", "");
            }

            // Get the TokenLoader instance
            var tokenLoader = _tokenLoader;

            // Get the cached token from the TokenLoader instance
            var cachedToken = tokenLoader.APITokens.FirstOrDefault(x => x.Key == token);

            // If the cached token is null, return an authentication failure
            if (cachedToken.Key is null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Token"));
            }

            // Create an array of claims for the authenticated user
            Claim[]? claims = new[]
            {
                    new Claim(type: "username", cachedToken.Value),
                    new Claim(type: "accountToken", cachedToken.Key),
                };

            // Generate a claims identity with the claims array and the main authentication scheme
            var claimsIdentity = new ClaimsIdentity(claims, Schemes.MainScheme);

            // Generate an authentication ticket from the claims identity and the current authentication scheme
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            // Pass the authentication ticket to the middleware
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
