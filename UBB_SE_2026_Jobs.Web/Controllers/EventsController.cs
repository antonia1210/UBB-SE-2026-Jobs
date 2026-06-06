namespace UBB_SE_2026_Jobs.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using UBB_SE_2026_Jobs.Library.ServiceProxies.Web;
    using UBB_SE_2026_Jobs.Library.DTOs.Web;

    [Authorize]
    public class EventsController : Controller
    {
        private readonly EventsApiClient _client;
        public EventsController(EventsApiClient client)
        {
            this._client = client;
        }

        private int GetCurrentUserId()
        {
            var claim = User.Claims.FirstOrDefault(claimItem => claimItem.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private int GetCurrentCompanyId()
        {
            var claim = User.Claims.FirstOrDefault(claimItem => claimItem.Type == "CompanyId");
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public async Task<IActionResult> Index()
        {
            List<EventDto> currentEvents;
            List<EventDto> pastEvents;

            if (User.IsInRole("Candidate"))
            {
                currentEvents = await this._client.GetAllCurrentEvents();
                pastEvents = await this._client.GetAllPastEvents();
            }
            else
            {
                int companyId = GetCurrentCompanyId();
                currentEvents = await this._client.GetCurrentEvents(companyId);
                pastEvents = await this._client.GetPastEvents(companyId);
            }

            ViewBag.CurrentEvents = currentEvents;
            ViewBag.PastEvents = pastEvents;

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            List<EventDto> currentEvents;
            List<EventDto> pastEvents;

            if (User.IsInRole("Candidate"))
            {
                currentEvents = await this._client.GetAllCurrentEvents();
                pastEvents = await this._client.GetAllPastEvents();
            }
            else
            {
                int companyId = GetCurrentCompanyId();
                currentEvents = await this._client.GetCurrentEvents(companyId);
                pastEvents = await this._client.GetPastEvents(companyId);
            }

            EventDto? ev = currentEvents
                .Concat(pastEvents)
                .FirstOrDefault(eventDto => eventDto.Id == id);

            if (ev == null)
            {
                return NotFound();
            }

            List<CompanyDto> collaborators = await this._client.GetEventCollaborators(id);
            ViewBag.Collaborators = collaborators;

            return View(ev);
        }

        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> Create()
        {
            var companies = await this._client.GetAllCompanies();
            int currentCompanyId = GetCurrentCompanyId();
            ViewBag.Companies = companies.Where(company => company.CompanyId != currentCompanyId).ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> Create(EventDto dto)
        {
            dto.HostCompanyId = GetCurrentCompanyId();
            dto.PostedAt = DateTime.Now;
            await this._client.Create(dto);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            int companyId = GetCurrentCompanyId();
            List<EventDto> currentEvents = await this._client.GetCurrentEvents(companyId);
            List<EventDto> pastEvents = await this._client.GetPastEvents(companyId);

            EventDto? ev = currentEvents.Concat(pastEvents).FirstOrDefault(eventDto => eventDto.Id == id);

            if (ev == null)
                return NotFound();

            var companies = await this._client.GetAllCompanies();
            ViewBag.Companies = companies.Where(company => company.CompanyId != companyId).ToList();

            return View(ev);
        }

        [HttpPost]
        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> Edit(int id, EventDto dto)
        {
            await this._client.Update(id, dto);
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            int companyId = GetCurrentCompanyId();
            List<EventDto> currentEvents = await this._client.GetCurrentEvents(companyId);
            List<EventDto> pastEvents = await this._client.GetPastEvents(companyId);

            EventDto? ev = currentEvents.Concat(pastEvents).FirstOrDefault(eventDto => eventDto.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RecruiterOrAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await this._client.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
