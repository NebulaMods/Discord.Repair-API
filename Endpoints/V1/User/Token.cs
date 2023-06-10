using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Requests.User;
using DiscordRepair.Api.Records.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
namespace DiscordRepair.Api.Endpoints.V1.User;

/// <summary>
/// Represents an API endpoint for handling authentication requests with captcha verification.
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
public class Token : ControllerBase
{
    /// <summary>
    /// Handles the authentication request with captcha verification.
    /// </summary>
    /// <param name="tokenRequest">The authentication request object.</param>
    /// <returns>The authentication result.</returns>
    [HttpPost("token")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult<Generic>> HandleAsync(TokenRequest tokenRequest)
    {
        // Check if the request is null
        if (tokenRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "Invalid parameters. Please try again."
            });
        }

        // Verify the captcha code
        var captchaResults = await Utilities.Miscallenous.VerifyCaptchaAsync(tokenRequest.captchaCode);
        if (captchaResults is null || captchaResults.success is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "Invalid captcha. Please try again."
            });
        }

        // Authenticate the user
        await using var database = new DatabaseContext();
        var user = await database.users
            .FirstOrDefaultAsync(x => x.username.ToLower().Equals(tokenRequest.user.ToLower())
                || x.email.ToLower().Equals(tokenRequest.user.ToLower()));
        return user is null
            ? (ActionResult<Generic>)NotFound(new Generic()
            {
                success = false,
                details = "User doesn't exist. Please try again."
            })
            : await Utilities.Miscallenous.VerifyHashAsync(tokenRequest.password, user.password) is false
            ? (ActionResult<Generic>)BadRequest(new Generic()
            {
                success = false,
                details = "Invalid password. Please try again."
            })
            : (ActionResult<Generic>)Ok(new Generic()
            {
                success = true,
                details = user.apiToken
            });
    }

}
//public record TokenRequest
//{
//    /// <summary>
//    /// The username or email of the user.
//    /// </summary>
//    public string User { get; init; }

//    /// <summary>
//    /// The password of the user.
//    /// </summary>
//    public string Password { get; init; }

//    /// <summary>
//    /// The captcha code entered by the user.
//    /// </summary>
//    public string CaptchaCode { get; init; }
//}

//public async Task<ActionResult<Generic>> HandleAsync(TokenRequest tokenRequest)
//{
//    if (tokenRequest is null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "invalid paramaters, please try again."
//        });
//    }

//    using var http = new HttpClient();
//    //var formContent = new Dictionary<string, string>
//    //{
//    //    { "response", userRequest.captchaCode },
//    //    { "secret", Properties.Resources.HCaptchaKey },
//    //    { "sitekey", "0d92223e-505f-4dd9-a808-55378fa9307c" }
//    //};
//    var formContent = new Dictionary<string, string>
//    {
//        { "response", tokenRequest.captchaCode },
//        { "secret", Properties.Resources.ReCaptchaKey },
//    };
//    var content = new FormUrlEncodedContent(formContent);
//    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
//    //var requestResults = await http.PostAsync($"https://hcaptcha.com/siteverify", content);
//    var requestResults = await http.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
//    var captchaResults = JsonConvert.DeserializeObject<ReCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
//    //var captchaResults = JsonConvert.DeserializeObject<HCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
//    if (captchaResults is null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "invalid captcha, please try again."
//        });
//    }
//    if (captchaResults.success is false)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "invalid captcha, please try again."
//        });
//    }
//    await using var database = new DatabaseContext();
//    var user = await database.users.FirstOrDefaultAsync(x => x.username.Equals(tokenRequest.user, StringComparison.OrdinalIgnoreCase) || x.email.Equals(tokenRequest.user, StringComparison.OrdinalIgnoreCase));
//    return user is null
//        ? NotFound(new Generic()
//        {
//            success = false,
//            details = "user doesn't exist, please try again."
//        })
//        : await Utilities.Miscallenous.VerifyHash(tokenRequest.password, user.password) is false
//        ? BadRequest(new Generic()
//        {
//            success = false,
//            details = "invalid password, please try again."
//        })
//        : Ok(new Generic()
//        {
//            success = true,
//            details = user.apiToken
//        });
//}

