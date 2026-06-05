using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Library.Services.PussyCatsCompanyService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/companies")]
public class PussyCatsCompaniesController : ControllerBase
{
    private readonly IPussyCatsCompanyService companyService;

    public PussyCatsCompaniesController(IPussyCatsCompanyService companyService)
    {
        this.companyService = companyService;
    }

    // Anonymous: the recruiter sign-up screen loads this list to populate the company
    // picker before the user is authenticated. The list is non-sensitive (public company
    // names/logos), so allow it without a token while the rest of the controller stays secured.
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Company>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var companies = await companyService.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return Ok(companies);
    }

    [HttpGet("{companyId:int}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int companyId, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByIdAsync(companyId, cancellationToken).ConfigureAwait(false);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Company), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] Company company, CancellationToken cancellationToken)
    {
        var createdCompany = await companyService.AddAsync(company, cancellationToken).ConfigureAwait(false);
        return CreatedAtRoute("GetById", new { companyId = createdCompany.CompanyId }, createdCompany);
    }

    [HttpPut("{companyId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int companyId, [FromBody] Company company, CancellationToken cancellationToken)
    {
        if (companyId != company.CompanyId)
            return BadRequest("Route companyId does not match the company identifier in the request body.");

        await companyService.UpdateAsync(company, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpDelete("{companyId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveAsync([FromRoute] int companyId, CancellationToken cancellationToken)
    {
        await companyService.RemoveAsync(companyId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}