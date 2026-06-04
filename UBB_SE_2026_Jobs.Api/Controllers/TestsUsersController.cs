namespace UBB_SE_2026_Jobs.Api.Controllers;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    [Route("api/[controller]")]
    [ApiController]
    public class TestsUsersController : ControllerBase
    {
        private readonly JobsDbContext dbContext;

        public TestsUsersController(JobsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await this.dbContext.Users.FindAsync(id);
            return user is null ? NotFound() : Ok(user.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await this.dbContext.Users.ToListAsync();
            return Ok(users.Select(u => u.ToDto()).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] UserDto dto)
        {
            this.dbContext.Users.Add(dto.ToEntity());
            await this.dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDto dto)
        {
            var user = dto.ToEntity();
            user.Id = id;
            this.dbContext.Entry(user).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await this.dbContext.Users.FindAsync(id);
            if (user is null) return NotFound();
            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
            return Ok(new { message = "User deleted successfully" });
        }
    }


