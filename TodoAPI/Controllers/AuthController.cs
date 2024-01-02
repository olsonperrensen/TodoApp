using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration config, ILogger<AuthController> logger)
    {
        _config = config;
        _logger = logger;
    }

    public record AuthenticationData(string? Username, string? Password);
    public record UserData(int Id,string FistName,string LastName, string UserName);
    /// <summary>
    /// Allows user to get a token by providing username and password
    /// </summary>
    /// <remarks>
    /// Sample request: POST api/Auth/token 
    /// Samples body: "username":"admin","password":"admin"
    /// Sample Response: 200 OK (with token as JSON body)
    /// </remarks>
    /// <returns>A JWT token string.</returns>
    /// <param name="data"></param>
    /// 
    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        try
        {
            _logger.LogInformation("POST api/Auth/token : Someone tried to token. Usr: {usr} Pwd: {pwd}",data.Username,data.Password);
            var user = ValidateCredentials(data);
            if (user == null)
            {
                return Unauthorized();
            }
            string token = GenerateToken(user);

            return Ok(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "POST api/Auth/token : Something went wrong. E:{e}",e);
            return BadRequest("Token went wrong...");
        }
    }

    private string GenerateToken(UserData user)
    {
        var GEHEIM = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            _config.GetValue<string>("Authentication:GEHEIM")!));

        var signingCredentials = new SigningCredentials(GEHEIM, SecurityAlgorithms.HmacSha256Signature);

        var clams = new List<Claim>() { 
            new(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName,user.UserName),
            new(JwtRegisteredClaimNames.GivenName,user.FistName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName)};

        var token = new JwtSecurityToken(_config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"), clams, DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(1), signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserData ValidateCredentials(AuthenticationData data)
    {
        if (CompareValues(data.Username, "admin") && CompareValues(data.Password, "admin"))
        {
            return new UserData(1,"admin","admin","admin");
        }
        return null!;
    }

    private bool CompareValues(string? actual, string expected)
    {
        if(actual is not null)
        {
            return actual.Equals(expected);
        }
        return false;
    }
}
