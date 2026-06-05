using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsUsersController : ControllerBase
{
    private readonly JobsDbContext databaseContext;

    public TestsUsersController(JobsDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetById(int userId)
    {
        var user = await this.databaseContext.Users.FindAsync(userId);
        return user is null ? NotFound() : Ok(user.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await this.databaseContext.Users.ToListAsync();
        return Ok(users.Select(user => user.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] UserDto userDto)
    {
        this.databaseContext.Users.Add(userDto.ToEntity());
        await this.databaseContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> Update(int userId, [FromBody] UserDto userDto)
    {
        var user = userDto.ToEntity();
        user.Id = userId;
        this.databaseContext.Entry(user).State = EntityState.Modified;
        await this.databaseContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Delete(int userId)
    {
        var user = await this.databaseContext.Users.FindAsync(userId);
        if (user is null) return NotFound();
        this.databaseContext.Users.Remove(user);
        await this.databaseContext.SaveChangesAsync();
        return Ok(new { message = "User deleted successfully" });
    }
}