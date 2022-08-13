using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database;
using DiscordRepair.Records.Requests.User;
using DiscordRepair.Records.Responses;
using Newtonsoft.Json;
using static DiscordRepair.Endpoints.V1.User.Create;

namespace DiscordRepair.Endpoints.V1.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
public class Token : ControllerBase
{
    /// <summary>
    /// Get your api token using your login credentials.
    /// </summary>
    /// <param name="tokenRequest"></param>
    /// <remarks>Get your api token using your login credentials.</remarks>
    /// <returns></returns>
    [HttpPost("token")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 200)]
    [ProducesResponseType(typeof(Generic), 400)]
    [ProducesResponseType(typeof(Generic), 404)]
    public async Task<ActionResult<Generic>> HandleAsync(TokenRequest tokenRequest)
    {
        if (tokenRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        using var http = new HttpClient();
        var formContent = new Dictionary<string, string>
        {
            { "response", tokenRequest.captchaCode },
            { "secret", Properties.Resources.HCaptchaKey },
            { "sitekey", "0d92223e-505f-4dd9-a808-55378fa9307c" }
        };
        var content = new FormUrlEncodedContent(formContent);
        //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        var requestResults = await http.PostAsync($"https://hcaptcha.com/siteverify", content);
        var captchaResults = JsonConvert.DeserializeObject<HCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
        if (captchaResults is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid captcha, please try again."
            });
        }
        if (captchaResults.success is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid captcha, please try again."
            });
        }
        await using var database = new DatabaseContext();
        var user = await database.users.FirstOrDefaultAsync(x => x.username == tokenRequest.user || x.email == tokenRequest.user);
        return user is null
            ? NotFound(new Generic()
            {
                success = false,
                details = "user doesn't exist, please try again."
            })
            : await Utilities.Miscallenous.VerifyHash(tokenRequest.password, user.password) is false
            ? BadRequest(new Generic()
            {
                success = false,
                details = "invalid password, please try again."
            })
            : Ok(new Generic()
            {
                success = true,
                details = user.apiToken
            });
    }

}
