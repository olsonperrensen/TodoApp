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

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    public record AuthenticationData(string? Username, string? Password);
    public record UserData(int Id,string FistName,string LastName, string UserName);

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);
        if(user == null)
        {
            return Unauthorized();
        }
        string token = GenerateToken(user);

        return Ok(token);
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
