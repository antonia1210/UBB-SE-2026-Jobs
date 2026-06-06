namespace UBB_SE_2026_Jobs.Web.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UBB_SE_2026_Jobs.Library.ServiceProxies.Web;
    using UBB_SE_2026_Jobs.Library.DTOs.Web;
    using UBB_SE_2026_Jobs.Web.Models;

    /// <summary>
    /// Controller responsible for the candidate test catalog and test-taking experience.
    /// </summary>
    public class TestsController : Controller
    {
        private readonly TestsApiClient _api;
        private readonly LeaderboardApiClient _leaderboardApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsController"/> class.
        /// </summary>
        /// <param name="api">The API client used to communicate with the backend test services.</param>
        /// <param name="leaderboardApi">The API client used to communicate with the backend leaderboard services.</param>
        public TestsController(TestsApiClient api, LeaderboardApiClient leaderboardApi)
        {
            this._api = api;
            this._leaderboardApi = leaderboardApi;
        }

        /// <summary>
        /// Displays the dashboard containing all available tests grouped by category.
        /// </summary>
        /// <returns>An asynchronous task returning the Index view with the TestsViewModel.</returns>
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Index()
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.TryParse(userIdClaim, out int parsedId) ? parsedId : -1;

            List<string> categories = await this._api.GetCategories();
            TestsViewModel viewModel = new TestsViewModel();

            // Fetch all user attempts in one call so we can mark previously-taken tests.
            HashSet<int> completedTestIds = new HashSet<int>();
            if (userId != -1 && User.IsInRole("Candidate"))
            {
                var userAttempts = await this._api.GetAttemptsByUserAsync(userId);
                foreach (var a in userAttempts)
                {
                    if (string.Equals(a.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                        completedTestIds.Add(a.TestId);
                }
            }

            foreach (string category in categories)
            {
                List<TestDto> tests = await this._api.GetByCategory(category);
                foreach (TestDto test in tests)
                {
                    viewModel.Tests.Add(new TestCardViewModel
                    {
                        TestId = test.Id,
                        Title = test.Title,
                        Category = test.Category,
                        QuestionTypeLabel = test.QuestionTypeLabel,
                        HasBeenTaken = completedTestIds.Contains(test.Id)
                    });
                }
            }

            return View(viewModel);
        }
        /// <summary>
        /// Retrieves and displays the details of a specific test.
        /// </summary>
        /// <param name="id">The unique identifier of the test.</param>
        /// <returns>An asynchronous task returning the Details view.</returns>
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Details(int id)
        {
            TestDto? test = await this._api.GetById(id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        /// <summary>
        /// Initiates the test-taking process for a candidate by redirecting them to the Take action.
        /// </summary>
        /// <param name="id">The unique identifier of the test to start.</param>
        /// <returns>A redirection to the Take action.</returns>
        [Authorize(Roles = "Candidate")]
        public IActionResult Start(int id)
        {
            // Redirect to the Take action which already implements the test-taking flow
            return RedirectToAction("Take", new { id });
        }

        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Take(int id)
        {
            TestDto? test = await this._api.GetById(id);
            if (test == null) return NotFound();

            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.TryParse(userIdClaim, out int parsedId) ? parsedId : 1;

            // Tests are replayable. Resume the active IN_PROGRESS attempt if one exists,
            // otherwise start a fresh one. Never block based on prior completed attempts.
            var activeAttempt = await this._api.GetAttemptByUserAndTestAsync(userId, id);
            if (activeAttempt == null)
            {
                await this._api.StartAttemptAsync(userId, id);
            }

            List<QuestionDto> questions = await this._api.GetQuestionsByTestIdAsync(id);
            TakeTestViewModel viewModel = new TakeTestViewModel
            {
                TestId = test.Id,
                Title = test.Title,
                Questions = questions
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<IActionResult> Submit(TakeTestViewModel model)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.TryParse(userIdClaim, out int parsedId) ? parsedId : 1;

            List<QuestionDto> questions = await this._api.GetQuestionsByTestIdAsync(model.TestId);

            int validAnswerCount = 0;
            if (model.Answers != null)
            {
                foreach (var kvp in model.Answers)
                {
                    if (kvp.Value != null && kvp.Value.Any(v => !string.IsNullOrWhiteSpace(v)))
                    {
                        validAnswerCount++;
                    }
                }
            }

            if (!ModelState.IsValid || validAnswerCount < questions.Count)
            {
                model.Questions = questions;
                ModelState.AddModelError(string.Empty, "You must provide an answer for every question before submitting.");
                return View("Take", model);
            }

            var attempt = await this._api.GetAttemptByUserAndTestAsync(userId, model.TestId);
            if (attempt == null)
            {
                return NotFound("Active test attempt not found.");
            }

            float maxPossibleScore = 0f;
            List<AnswerDto> gradedAnswers = new List<AnswerDto>();

            foreach (var kvp in model.Answers ?? new Dictionary<int, List<string>>())
            {
                var q = questions.FirstOrDefault(x => x.Id == kvp.Key);
                if (q == null) continue;

                string joinedAnswer = string.Join(",", kvp.Value.Where(v => !string.IsNullOrWhiteSpace(v)));
                if (string.IsNullOrEmpty(joinedAnswer)) continue;

                var gradeRequest = new
                {
                    TestQuestion = new
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        QuestionTypeString = q.QuestionType,
                        QuestionScore = q.QuestionScore,
                        QuestionAnswer = q.QuestionAnswer
                    },
                    Answer = new
                    {
                        QuestionId = q.Id,
                        AttemptId = attempt.Id,
                        Value = joinedAnswer
                    }
                };

                AnswerDto gradedAnswer = await this._api.GradeAnswerAsync(q.QuestionType, gradeRequest);
                await this._api.SaveAnswerAsync(gradedAnswer);
                gradedAnswers.Add(gradedAnswer);
            }

            foreach (var q in questions)
            {
                maxPossibleScore += q.QuestionScore;
            }

            var scorePayload = new
            {
                Id = attempt.Id,
                Answers = gradedAnswers.Select(a => new { Value = a.Value }).ToList()
            };

            float rawScore = await this._api.CalculateFinalScoreAsync(scorePayload);

            attempt.Status = "COMPLETED";
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.Score = (decimal)rawScore;
            attempt.IsValidated = true;

            if (maxPossibleScore > 0)
            {
                attempt.PercentageScore = (decimal)((rawScore / maxPossibleScore) * 100f);
            }

            await this._api.UpdateAttemptAsync(attempt.Id, attempt);

            // Recalculate the leaderboard for this test now that the attempt is completed
            await this._leaderboardApi.RecalculateLeaderboardAsync(model.TestId);

            return RedirectToAction("Result", new { score = rawScore, maxScore = maxPossibleScore });
        }

        /// <summary>
        /// Displays the final calculated score to the candidate after a successful test submission.
        /// </summary>
        /// <param name="score">The final percentage score returned by the grading API.</param>
        /// <returns>The Result view displaying the final score.</returns>
        [Authorize(Roles = "Candidate")]
        public IActionResult Result(float score, float maxScore)
        {
            ViewBag.MaxScore = maxScore;
            return View(score);
        }
    }
}
