using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Infrastructure.Identity;
using UserManagement.Infrastructure.Security;

namespace UserManagement.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IAuthService authService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Forename = dto.FirstName,
            Surname = dto.LastName,
            IsActive = true
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(user);
        var (token, exp) = _tokenService.CreateToken(user, roles);
        return new AuthResultDto(token, exp);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _authService.ValidateCredentials(dto.Email, dto.Password);
        if (user == null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var (token, exp) = _tokenService.CreateToken(user, roles);

        return Ok(new AuthResultDto(token, exp));
    }


    [Authorize]
    [HttpGet("whoami")]
    public IActionResult WhoAmI() =>
        Ok(new { name = User.Identity?.Name, claims = User.Claims.Select(c => new { c.Type, c.Value }) });

}
