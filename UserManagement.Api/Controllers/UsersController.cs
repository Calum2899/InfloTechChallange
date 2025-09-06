using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Abstractions;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;
    public UsersController(IUserRepository repo) => _repo = repo;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> Get([FromQuery] bool? isActive, CancellationToken ct)
    {
        var items = await _repo.SearchAsync(isActive, ct);
        return items.Select(u => new UserDto(u.Id, u.Forename, u.Surname, u.Email, u.IsActive, u.DateOfBirth)).ToList();
    }

    [Authorize]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDto>> GetById(long id, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        return u is null
            ? NotFound()
            : new UserDto(u.Id, u.Forename, u.Surname, u.Email, u.IsActive, u.DateOfBirth);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<long>> Create(CreateUserDto dto, CancellationToken ct)
    {
        var entity = new User
        {
            Forename = dto.Forename,
            Surname = dto.Surname,
            Email = dto.Email,
            IsActive = dto.IsActive,
            DateOfBirth = dto.DateOfBirth
        };
        var id = await _repo.CreateAsync(entity, -1, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateUserDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        existing.Forename = dto.Forename;
        existing.Surname = dto.Surname;
        existing.Email = dto.Email;
        existing.IsActive = dto.IsActive;
        existing.DateOfBirth = dto.DateOfBirth;

        await _repo.UpdateAsync(existing, -1, ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _repo.DeleteAsync(id, -1, ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id:long}/logs")]
    public async Task<ActionResult<List<LogDto>>> GetLogs(long id, int skip = 0, int take = 50, CancellationToken ct = default)
    {
        var logs = await _repo.GetLogsForUserAsync(id, skip, take, ct);

        var result = logs.Select(l => new LogDto(
            l.Id,
            l.UserId,
            l.Action,
            l.Description,
            l.ModifiedBy,
            l.Timestamp
        )).ToList();

        return Ok(result);
    }

}
