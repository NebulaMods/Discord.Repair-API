
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using DiscordRepair.Database;
using DiscordRepair.Records.Requests.User;
using DiscordRepair.Records.Responses;
using DiscordRepair.Services;

namespace DiscordRepair.Endpoints.V1.User;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
public class Create : ControllerBase
{
    private readonly TokenLoader _tokenLoader;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenLoader"></param>
    public Create(TokenLoader tokenLoader)
    {
        _tokenLoader = tokenLoader;
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="userRequest"></param>
    /// <remarks>Create a new user.</remarks>
    /// <returns></returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 201)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandlesAsync(CreateUserRequest userRequest)
    {
        if (userRequest is null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "invalid paramaters, please try again."
            });
        }
        using var http = new HttpClient();
        http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type","application/x-www-form-urlencoded");
        var requestResults = await http.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={Properties.Resources.ReCaptchaKey}&response={userRequest.captchaCode}", null);
        var captchaResults = JsonConvert.DeserializeObject<ReCaptchaResponse>(await requestResults.Content.ReadAsStringAsync());
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
        var user = await database.users.FirstOrDefaultAsync(x => x.username == userRequest.username || x.email == userRequest.email);
        if (user is not null)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "user already exists, please try again."
            });
        }
        var newUser = new Database.Models.User()
        {
            email = userRequest.email,
            password = await Utilities.Miscallenous.HashPassword(userRequest.password),
            username = userRequest.username,
        };
        await database.users.AddAsync(newUser);
        await database.ApplyChangesAsync();
        _tokenLoader.APITokens.TryAdd(newUser.apiToken, newUser.username);
        return Created($"https://discord.repair/v1/user/{userRequest.username}", newUser.apiToken);
    }

    public record ReCaptchaResponse
    {
        public bool success { get; set; }
        public DateTime? challenge_ts { get; set; }
        public string? hostname { get; set; }
        [JsonProperty("error-codes")]
        public List<string?>? ErrorCodes { get; set; }
    }
}
