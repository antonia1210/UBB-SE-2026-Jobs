// <copyright file="InterviewsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Web.Clients;
    using UBB_SE_2026_Jobs.Web.Dtos;
    using UBB_SE_2026_Jobs.Library.Services.UserStatusService;

    /// <summary>
    /// Handles interview slot browsing and booking (candidates) and
    /// booked interview management (recruiters).
    /// </summary>
    public class InterviewsController : Controller
    {
        private readonly SlotsApiClient slotsClient;
        private readonly InterviewSessionsApiClient sessionsClient;
        private readonly JobsApiClient jobsClient;
        private readonly IUserStatusService userStatusService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterviewsController"/> class.
        /// </summary>
        public InterviewsController(
            SlotsApiClient slotsClient,
            InterviewSessionsApiClient sessionsClient,
            JobsApiClient jobsClient,
            IUserStatusService userStatusService)
        {
            this.slotsClient = slotsClient;
            this.sessionsClient = sessionsClient;
            this.jobsClient = jobsClient;
            this.userStatusService = userStatusService;
        }

        // ---------------------------------------------------------------
        // Candidate: browse available slots
        // ---------------------------------------------------------------

        /// <summary>
        /// Displays available interview slots for a recruiter on a given date.
        /// Accessible by candidates only.
        /// </summary>
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> AvailableSlots()
        {
            int candidateId = int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await this.userStatusService.GetApplicationsForUserAsync(candidateId);

            return this.View(applications);
        }

        /// <summary>
        /// Retrieves all available interview slots for a chosen job.
        /// </summary>
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetSlotsByJob(int jobId, DateTime? date)
        {
            var job = await this.jobsClient.GetJobByIdAsync(jobId);
            if (job == null) return NotFound();

            var slots = await this.slotsClient.GetAvailableSlotsForCompany(job.CompanyId, date ?? DateTime.Today);
            return Json(slots);
        }

        // ---------------------------------------------------------------
        // Candidate: book a slot
        // ---------------------------------------------------------------

        /// <summary>
        /// Confirms the booking of a slot for the currently logged-in candidate.
        /// Accessible by candidates only.
        /// </summary>
        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<IActionResult> BookSlot(int slotId, int jobId)
        {
            int candidateId = int.Parse(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await this.sessionsClient.ConfirmBookingAsync(slotId, candidateId, jobId);
                this.TempData["Success"] = "Interview slot booked successfully!";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                this.TempData["Error"] = "This slot is no longer available.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                this.TempData["Error"] = "Slot not found.";
            }

            return this.RedirectToAction(nameof(this.AvailableSlots));
        }
        // ---------------------------------------------------------------
        // Candidate: browse your booked interview sessions
        // ---------------------------------------------------------------

        /// <summary>
        /// Displays booked interview slots for logged in candidate.
        /// Accessible by candidates only.
        /// </summary>
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CandidateBookedInterviews()
        {
            int candidateId = int.Parse(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var booked = await this.sessionsClient.GetBookedByCandidate(candidateId); ;

            return View(booked);
        }

        // ---------------------------------------------------------------
        // Recruiter: view booked interviews
        // ---------------------------------------------------------------

        /// <summary>
        /// Displays all scheduled interview sessions for the recruiter.
        /// Accessible by recruiters only.
        /// </summary>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> BookedInterviews()
        {
            int recruiterId = int.Parse(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var sessions = await this.sessionsClient.GetScheduledAsync(recruiterId);
            return this.View(sessions);
        }

        // ---------------------------------------------------------------
        // Recruiter: manage interview slots
        // ---------------------------------------------------------------
        /// <summary>
        /// Displaying page for managing slots and sets the list of companies for logged in recruiter.
        /// </summary>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> ManageSlots()
        {
            int recruiterId = int.Parse(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            List<CompanyDto> companies = await this.slotsClient.GetCompaniesForRecruiterAsync(recruiterId);
            ViewData["Companies"] = companies;

            return this.View();
        }

        /// <summary>
        /// Get all slots of the logged in recruiter to display them.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetRecruiterSlots()
        {
            int recruiterId= int.Parse(
                this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            List<SlotDto> slots = await this.slotsClient.GetAllByRecruiterAsync(recruiterId);

            return Json(slots);
        }

        /// <summary>
        /// Create a new slot for the logged in recruiter.
        /// </summary>
        /// <param name="baseSlot">base slot with date, start time and company</param>
        /// <param name="duration">duration of slot</param>
        [HttpPost]
        public async Task<IActionResult> CreateRecruiterSlot(SlotDto baseSlot, int duration)
        {
            try {
                int recruiterId = int.Parse(
                    this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                baseSlot.RecruiterId = recruiterId;

                await this.slotsClient.AddRecruiterSlotAsync(baseSlot, duration);
                return Ok();
            } catch (HttpRequestException) {
                this.TempData["Error"] = "Slot not available.";
            }

            return this.RedirectToAction(nameof(this.ManageSlots));
        }

        /// <summary>
        /// Update a slot of the logged in recruiter.
        /// </summary>
        /// <param name="initialSlot">initial slot to change</param>
        /// <param name="startTime">new start time</param>
        /// <param name="duration">new duration</param>
        [HttpPost]
        public async Task<IActionResult> UpdateRecruiterSlot(SlotDto initialSlot, DateTime startTime, int duration)
        {
            try {
                await this.slotsClient.UpdateRecruiterSlotAsync(initialSlot, startTime, duration);
                return Ok();
            } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
                this.TempData["Error"] = "Slot not found.";
            } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                this.TempData["Error"] = "Could not update slot.";
            }

            return this.RedirectToAction(nameof(this.ManageSlots));
        }

        /// <summary>
        /// Delete a recruiter slot.
        /// </summary>
        /// <param name="id">id of slot to delete</param>
        [HttpPost]
        public async Task<IActionResult> DeleteRecruiterSlot(int id)
        {
            try {
                await this.slotsClient.DeleteRecruiterSlotAsync(id);
                return Ok();
            } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
                this.TempData["Error"] = "Slot not found.";
            } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                this.TempData["Error"] = "Could not delete slot.";
            }

            return this.RedirectToAction(nameof(this.ManageSlots));
        }

        // ---------------------------------------------------------------
        // Recruiter: accept or decline candidate interview
        // ---------------------------------------------------------------

        /// <summary>
        /// Submits the recruiter's accept/reject decision for a candidate.
        /// </summary>
        [Authorize(Roles = "Recruiter")]
        [HttpPost]
        public async Task<IActionResult> SubmitDecision(int sessionId, string decision)
        {
            try {
                await this.sessionsClient.SetInterviewDecision(sessionId, decision);
            } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
                this.TempData["Error"] = "Application not found.";
            } catch (HttpRequestException) {
                this.TempData["Error"] = "Failed to submit decision.";
            }

            return this.RedirectToAction(nameof(this.BookedInterviews));
        }
    }
}
