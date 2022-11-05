using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DiscordRepair.Middleware;

/// <summary>
/// 
/// </summary>
public class Authentication
{
    internal static class Schemes
    {
        internal const string MainScheme = "MainScheme";
    }
    /// <summary>
    /// 
    /// </summary>
    public class Handler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Services.TokenLoader _tokenLoader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="tokenLoader"></param>
        public Handler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, Services.TokenLoader tokenLoader) : base(options, logger, encoder, clock) => _tokenLoader = tokenLoader;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization") is false)
                return Task.FromResult(AuthenticateResult.NoResult());

            string? token = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Token"));

            if (token.Contains("Authorization "))
            {
                token = token.Replace("Authorization ", "");
            }
            var tokenLoader = _tokenLoader;
            var cachedToken = tokenLoader.APITokens.FirstOrDefault(x => x.Key == token);
            if (cachedToken.Key is null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Token"));
            }

            Claim[]? claims = new[]
            {
                new Claim(type: "username", cachedToken.Value),
            };

            // generate claimsIdentity on the name of the class
            var claimsIdentity = new ClaimsIdentity(claims, Schemes.MainScheme);

            // generate AuthenticationTicket from the Identity and current authentication scheme
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            // pass on the ticket to the middleware
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
