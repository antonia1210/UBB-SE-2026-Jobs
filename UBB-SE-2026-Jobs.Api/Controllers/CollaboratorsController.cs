namespace UBB_SE_2026_Jobs.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Services.Portal;

    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorsController : ControllerBase
    {
        private readonly ICollaboratorsService _service;

        public CollaboratorsController(ICollaboratorsService service)
        {
            this._service = service;
        }

        [HttpPost]
        public ActionResult AddCollaborator([FromBody] CollaboratorDto collaboratorDto, [FromQuery] int loggedInUserID)
        {
            Event eventOfCollaboration = collaboratorDto.EventId > 0
                ? new Event { Id = collaboratorDto.EventId }
                : new Event();

            Company collaboratorToBeAdded = new Company { CompanyId = collaboratorDto.CompanyId };

            this._service.AddCollaboratorToRepo(eventOfCollaboration, collaboratorToBeAdded, loggedInUserID);

            return Ok();
        }

        [HttpGet("{loggedInCompanyId}")]
        public ActionResult<List<CompanyDto>> GetAllCollaborators(int loggedInCompanyId)
        {
            List<Company> collaborators = this._service.GetAllCollaborators(loggedInCompanyId);

            if (collaborators is null || !collaborators.Any())
                return NotFound($"No collaborators found for company ID {loggedInCompanyId}.");

            return Ok(collaborators.Select(c => c.ToDto()).ToList());
        }
    }
}