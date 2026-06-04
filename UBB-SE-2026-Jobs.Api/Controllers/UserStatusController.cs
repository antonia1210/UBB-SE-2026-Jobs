using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/user-status")]
public class UserStatusController : ControllerBase
{
    private readonly IUserStatusService userStatus;

    public UserStatusController(IUserStatusService userStatus)
    {
        this.userStatus = userStatus;
    }

    [HttpGet("{userId}/applications")]
    public async Task<IActionResult> GetApplications(int userId, CancellationToken cancellationToken)
        => Ok(await userStatus.GetApplicationsForUserAsync(userId, cancellationToken));
}
