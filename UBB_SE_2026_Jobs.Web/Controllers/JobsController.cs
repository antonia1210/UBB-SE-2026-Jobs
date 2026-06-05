namespace UBB_SE_2026_Jobs.Web.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UBB_SE_2026_Jobs.Library.Services;
    using UBB_SE_2026_Jobs.Web.Clients;
    using UBB_SE_2026_Jobs.Web.Dtos;
    using UBB_SE_2026_Jobs.Web.Infrastructure;

    /// <summary>
    /// Handles all job-related pages. Delegates all data operations to <see cref="JobsApiClient"/>.
    /// </summary>
    [Authorize]
    public class JobsController : Controller
    {
        private readonly JobsApiClient jobsApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobsController"/> class.
        /// </summary>
        /// <param name="jobsApiClient">The API proxy client for jobs.</param>
        public JobsController(JobsApiClient jobsApiClient)
        {
            this.jobsApiClient = jobsApiClient;
        }

        /// <summary>
        /// Displays the job postings for the recruiter's company.
        /// </summary>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Index()
        {
            this.AttachJwt();
            var companyIdClaim = this.User.FindFirstValue("CompanyId");
            if (int.TryParse(companyIdClaim, NumberStyles.Integer, CultureInfo.InvariantCulture, out int companyId))
            {
                List<JobPostingDto> jobs = await this.jobsApiClient.GetJobsByCompanyIdAsync(companyId);
                return this.View(jobs);
            }

            return this.Forbid();
        }

        /// <summary>
        /// Displays the details of a single job posting.
        /// </summary>
        /// <param name="id">The job posting ID.</param>
        public async Task<IActionResult> Details(int id)
        {
            this.AttachJwt();
            JobPostingDto? job = await this.jobsApiClient.GetJobByIdAsync(id);
            if (job == null)
            {
                return this.NotFound();
            }

            return this.View(job);
        }

        /// <summary>
        /// Displays the create job form. Recruiter only.
        /// </summary>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Create()
        {
            this.AttachJwt();
            List<SkillDto> skills = await this.jobsApiClient.GetAllSkillsAsync();
            this.ViewBag.Skills = skills;
            return this.View(new JobPostingDto());
        }

        /// <summary>
        /// Handles create job form submission. Recruiter only.
        /// </summary>
        /// <param name="dto">The job posting form data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Create(JobPostingDto dto)
        {
            this.ModelState.Remove("JobSkills");

            if (!this.ModelState.IsValid)
            {
                this.AttachJwt();
                this.ViewBag.Skills = await this.jobsApiClient.GetAllSkillsAsync();
                return this.View(dto);
            }

            this.AttachJwt();

            // Filter JobSkills to only include selected ones (where SkillId > 0)
            var selectedSkills = dto.JobSkills
                .Where(js => js.SkillId > 0 && js.RequiredPercentage > 0)
                .ToList();

            // Clear JobSkills from the DTO before sending to API
            // The skills will be handled separately via SkillLinks
            dto.JobSkills.Clear();

            AddJobDto addDto = new AddJobDto
            {
                Job = dto,
                UserId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                SkillLinks = selectedSkills,
            };
            Debug.WriteLine(addDto.UserId);

            bool success = await this.jobsApiClient.AddJobAsync(addDto);
            if (!success)
            {
                this.ModelState.AddModelError(string.Empty, "Failed to create job. Please try again.");
                this.ViewBag.Skills = await this.jobsApiClient.GetAllSkillsAsync();
                return this.View(dto);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Displays the edit job form. Recruiter only.
        /// </summary>
        /// <param name="id">The job posting ID.</param>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Edit(int id)
        {
            this.AttachJwt();
            JobPostingDto? job = await this.jobsApiClient.GetJobByIdAsync(id);
            if (job == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Skills = await this.jobsApiClient.GetAllSkillsAsync();
            return this.View(job);
        }

        /// <summary>
        /// Handles edit job form submission. Recruiter only.
        /// </summary>
        /// <param name="id">The job posting ID.</param>
        /// <param name="dto">The updated job data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Edit(int id, JobPostingDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                this.AttachJwt();
                this.ViewBag.Skills = await this.jobsApiClient.GetAllSkillsAsync();
                return this.View(dto);
            }

            this.AttachJwt();
            bool success = await this.jobsApiClient.UpdateJobAsync(id, dto);
            if (!success)
            {
                this.ModelState.AddModelError(string.Empty, "Failed to update job. Please try again.");
                this.ViewBag.Skills = await this.jobsApiClient.GetAllSkillsAsync();
                return this.View(dto);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Displays the delete confirmation page. Recruiter only.
        /// </summary>
        /// <param name="id">The job posting ID.</param>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Delete(int id)
        {
            this.AttachJwt();
            JobPostingDto? job = await this.jobsApiClient.GetJobByIdAsync(id);
            if (job == null)
            {
                return this.NotFound();
            }

            this.ViewBag.ApplicantCount = await this.jobsApiClient.GetApplicantCountAsync(id);
            return this.View(job);
        }

        /// <summary>
        /// Handles confirmed deletion of a job posting. Recruiter only.
        /// </summary>
        /// <param name="id">The job posting ID.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteConfirmed(int id, bool force)
        {
            this.AttachJwt();
            JobDeleteResult result = await this.jobsApiClient.DeleteJobAsync(id, force);

            if (result == JobDeleteResult.Deleted)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            if (result == JobDeleteResult.NotFound)
            {
                return this.NotFound();
            }
            
            JobPostingDto? job = await this.jobsApiClient.GetJobByIdAsync(id);
            if (job == null)
            {
                return this.NotFound();
            }

            this.ViewBag.ApplicantCount = await this.jobsApiClient.GetApplicantCountAsync(id);
            return this.View(job);
        }

        /// <summary>
        /// Reads the main API JWT token from session and attaches it to the API client.
        /// </summary>
        private void AttachJwt()
        {
            string? jwt = this.HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (!string.IsNullOrEmpty(jwt))
            {
                this.jobsApiClient.SetAuthToken(jwt);
            }
        }
    }
}
