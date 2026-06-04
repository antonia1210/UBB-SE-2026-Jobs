namespace UBB_SE_2026_Jobs.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Services.Portal;

    [Route("api/portal/companies")]
    [ApiController]
    public class PortalCompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            this._service = service;
        }

        [HttpGet]
        public ActionResult<List<CompanyDto>> GetAll()
        {
            List<Company> companies = this._service.GetAll();

            return Ok(companies.Select(c => c.ToDto()).ToList());
        }

        [HttpGet("{companyId}")]
        public ActionResult<CompanyDto> GetById(int companyId)
        {
            Company? company = this._service.GetById(companyId);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company.ToDto());
        }

        [HttpGet("byname/{companyName}")]
        public ActionResult<CompanyDto> GetByName(string companyName)
        {
            Company? company = this._service.GetCompanyByName(companyName);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company.ToDto());
        }

        [HttpPost]
        public ActionResult<CompanyDto> Add([FromBody] CompanyDto dto)
        {
            Company company = dto.ToEntity();
            this._service.Add(company);

            return Ok(company.ToDto());
        }

        [HttpPut("{companyId}")]
        public ActionResult<CompanyDto> Update(int companyId, [FromBody] CompanyDto dto)
        {
            Company company = dto.ToEntity();
            company.CompanyId = companyId;
            this._service.Update(company);

            return Ok(company.ToDto());
        }

        [HttpDelete("{companyId}")]
        public ActionResult Remove(int companyId)
        {
            this._service.Remove(companyId);

            return Ok(new { message = "Company removed successfully" });
        }

        [HttpGet("{companyId}/game")]
        public ActionResult<GameDto> GetGame(int companyId)
        {
            GameDto? gameDto = this._service.GetGame(companyId);
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
                this._service.SaveGame(companyId, gameDto);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}