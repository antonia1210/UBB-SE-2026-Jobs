using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsCompaniesController : ControllerBase
{
    private readonly ITestsCompanyService testsCompanyService;

    public TestsCompaniesController(ITestsCompanyService testsCompanyService)
    {
        this.testsCompanyService = testsCompanyService;
    }

    [HttpGet]
    public ActionResult<List<CompanyDto>> GetAll()
    {
        List<Company> companies = this.testsCompanyService.GetAll();
        return Ok(companies.Select(company => company.ToDto()).ToList());
    }

    [HttpGet("{companyId}")]
    public ActionResult<CompanyDto> GetById(int companyId)
    {
        Company? company = this.testsCompanyService.GetById(companyId);

        if (company == null)
        {
            return NotFound();
        }

        return Ok(company.ToDto());
    }

    [HttpGet("byname/{companyName}")]
    public ActionResult<CompanyDto> GetByName(string companyName)
    {
        Company? company = this.testsCompanyService.GetCompanyByName(companyName);

        if (company == null)
        {
            return NotFound();
        }

        return Ok(company.ToDto());
    }

    [HttpGet("byrecruiter/{recruiterId}")]
    public ActionResult<List<CompanyDto>> GetByRecruiter(int recruiterId)
    {
        List<Company> recruiterCompanies = this.testsCompanyService.GetByRecruiter(recruiterId);
        return Ok(recruiterCompanies.Select(company => company.ToDto()).ToList());
    }

    [HttpPost]
    public ActionResult<CompanyDto> Add([FromBody] CompanyDto companyDto)
    {
        Company company = companyDto.ToEntity();
        this.testsCompanyService.Add(company);
        return Ok(company.ToDto());
    }

    [HttpPut("{companyId}")]
    public ActionResult<CompanyDto> Update(int companyId, [FromBody] CompanyDto companyDto)
    {
        Company company = companyDto.ToEntity();
        company.CompanyId = companyId;
        this.testsCompanyService.Update(company);
        return Ok(company.ToDto());
    }

    [HttpDelete("{companyId}")]
    public ActionResult Remove(int companyId)
    {
        this.testsCompanyService.Remove(companyId);
        return Ok(new { message = "Company removed successfully" });
    }

    [HttpGet("{companyId}/game")]
    public ActionResult<GameDto> GetGame(int companyId)
    {
        GameDto? gameDto = this.testsCompanyService.GetGame(companyId);
        if (gameDto == null)
        {
            return NotFound();
        }
        return Ok(gameDto);
    }

    [HttpPut("{companyId}/game")]
    public ActionResult SaveGame(int companyId, [FromBody] GameDto gameDto)
    {
        try
        {
            this.testsCompanyService.SaveGame(companyId, gameDto);
            return Ok();
        }
        catch (InvalidOperationException invalidOperationException)
        {
            return NotFound(invalidOperationException.Message);
        }
    }
}