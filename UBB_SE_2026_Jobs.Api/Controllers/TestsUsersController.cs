using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Repositories.Users;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsUsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public TestsUsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetById(int userId)
    {
        var user = await this.userRepository.GetByIdAsync(userId);
        return user is null ? NotFound() : Ok(user.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await this.userRepository.GetAllAsync();
        return Ok(users.Select(user => user.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] UserDto userDto)
    {
        await this.userRepository.AddAsync(userDto.ToEntity());
        return Ok();
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> Update(int userId, [FromBody] UserDto userDto)
    {
        var user = userDto.ToEntity();
        user.Id = userId;
        await this.userRepository.UpdateAsync(user);
        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Delete(int userId)
    {
        var user = await this.userRepository.GetByIdAsync(userId);
        if (user is null) return NotFound();
        await this.userRepository.RemoveAsync(userId);
        return Ok(new { message = "User deleted successfully" });
    }
}
