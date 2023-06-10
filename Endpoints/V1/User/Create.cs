using System.Text.RegularExpressions;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Records.Requests.User;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Services;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace DiscordRepair.Api.Endpoints.V1.User;

/// <summary>
/// Represents the controller responsible for creating a new user account.
/// </summary>
[ApiController]
[Route("/v1/user/")]
[ApiExplorerSettings(GroupName = "Account Endpoints")]
[AllowAnonymous]
public class Create : ControllerBase
{
    private readonly TokenLoader _tokenLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="Create"/> class with the specified token loader.
    /// </summary>
    /// <param name="tokenLoader">The token loader to use for authentication.</param>
    public Create(TokenLoader tokenLoader)
    {
        _tokenLoader = tokenLoader;
    }

    /// <summary>
    /// Handles a PUT request to create a new user.
    /// </summary>
    /// <param name="userRequest">The user information to create a new user</param>
    /// <returns>Returns a JSON response indicating whether the operation was successful or not.</returns>
    [HttpPut]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Generic), 201)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Generic>> HandlesAsync(CreateUserRequest userRequest)
    {
        // Check if the username provided is valid
        if (string.IsNullOrEmpty(userRequest?.username) || !Utilities.Miscallenous.IsValidUsername(userRequest.username))
        {
            return BadRequest(new Generic
            {
                success = false,
                details = "Invalid username, please try again."
            });
        }

        // Check if the captcha is valid
        var captcha = await Utilities.Miscallenous.VerifyCaptchaAsync(userRequest.captchaCode);
        if (captcha is null || captcha.success is false)
        {
            return BadRequest(new Generic
            {
                success = false,
                details = "Invalid captcha, please try again."
            });
        }

        await using var database = new DatabaseContext();

        // Check if a user already exists with the provided username or email address
        var existingUser = await Utilities.Miscallenous.GetUserByUsernameOrEmailAsync(userRequest.username) ?? await Utilities.Miscallenous.GetUserByUsernameOrEmailAsync(userRequest.email);
        if (existingUser != null)
        {
            return BadRequest(new Generic
            {
                success = false,
                details = "User already exists, please try again."
            });
        }

        // Create a new user with the provided information
        var newUser = new Database.Models.User
        {
            email = userRequest.email,
            password = await Miscallenous.HashPasswordAsync(userRequest.password),
            username = userRequest.username,
            lastIP = HttpContext.GetIPAddress(),
            apiToken = Guid.NewGuid().ToString()
        };

        // Add the new user to the database
        await database.users.AddAsync(newUser);
        await database.ApplyChangesAsync();

        // Add the new user's API token to the token loader
        _tokenLoader.APITokens.TryAdd(newUser.apiToken, newUser.username);

        // Return a successful response with the new user's API token
        return Created($"https://api.discord.repair/v1/user/{userRequest.username}", new Generic
        {
            success = true,
            details = newUser.apiToken
        });
    }

}
//public async Task<ActionResult<Generic>> HandlesAsync(CreateUserRequest userRequest)
//{
//    if (userRequest is null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "invalid paramaters, please try again."
//        });
//    }

//    //check special chars
//    if (Regex.IsMatch(userRequest.username, "^[-a-zA-Z0-9-()]+(\\s+[-a-zA-Z0-9-()]+)*$") is false)
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
//        { "response", userRequest.captchaCode },
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
//    var tokenLoader = _tokenLoader;
//    var user = await database.users.FirstOrDefaultAsync(x => x.username == userRequest.username || x.email == userRequest.email);
//    if (user is not null)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "user already exists, please try again."
//        });
//    }
//    var newUser = new Database.Models.User()
//    {
//        email = userRequest.email,
//        password = await Miscallenous.HashPasswordAsync(userRequest.password),
//        username = userRequest.username,
//        lastIP = HttpContext.GetIPAddress()
//    };
//    await database.users.AddAsync(newUser);
//    await database.ApplyChangesAsync();
//    tokenLoader.APITokens.TryAdd(newUser.apiToken, newUser.username);
//    return Created($"https://api.discord.repair/v1/user/{userRequest.username}", new Generic()
//    {
//        success = true,
//        details = newUser.apiToken
//    });
//}
