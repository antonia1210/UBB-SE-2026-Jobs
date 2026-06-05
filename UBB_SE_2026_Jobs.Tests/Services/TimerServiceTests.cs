using NSubstitute;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Helpers;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services
{
    public class TimerServiceTests
    {
        private readonly ITestAttemptRepository testAttemptRepository;
        private readonly TimerService timerService;

        public TimerServiceTests()
        {
            testAttemptRepository = Substitute.For<ITestAttemptRepository>();
            timerService = new TimerService(testAttemptRepository);

            // Clear the static ConcurrentDictionary between tests to ensure isolation
            ClearStaticTimers();
        }

        public void Dispose()
        {
            ClearStaticTimers();
        }

        private void ClearStaticTimers()
        {
            var field = typeof(TimerService).GetField("Timers", BindingFlags.Static | BindingFlags.NonPublic);
            if (field?.GetValue(null) is ConcurrentDictionary<int, DateTime> timers)
            {
                timers.Clear();
            }
        }


        [Fact]
        public void CheckExpiration_ShouldReturnFalse_WhenTimerDoesNotExist()
        {
            // Arrange
            int nonExistentAttemptId = 999;

            // Act
            bool isExpired = timerService.CheckExpiration(nonExistentAttemptId);

            // Assert
            Assert.False(isExpired);
        }

        [Fact]
        public void CheckExpiration_ShouldReturnFalse_WhenTimerHasNotExpired()
        {
            // Arrange
            int attemptId = 1;
            timerService.StartTimer(attemptId); // Starts right now, well within the limit

            // Act
            bool isExpired = timerService.CheckExpiration(attemptId);

            // Assert
            Assert.False(isExpired);
        }

        [Fact]
        public void CheckExpiration_ShouldReturnTrue_WhenTimerHasExpired()
        {
            // Arrange
            int attemptId = 2;
            timerService.StartTimer(attemptId);

            // Backdate the timer manually using reflection to simulate expiration
            BackdateTimer(attemptId, TestConstants.TestDurationInMinutes + 5);

            // Act
            bool isExpired = timerService.CheckExpiration(attemptId);

            // Assert
            Assert.True(isExpired);
        }


        [Fact]
        public void GetExpiredAttemptIds_ShouldReturnOnlyExpiredIds()
        {
            // Arrange
            int activeAttemptId = 10;
            int expiredAttemptId = 20;

            timerService.StartTimer(activeAttemptId);  // Active
            timerService.StartTimer(expiredAttemptId); // Will be expired

            // Backdate only the second attempt
            BackdateTimer(expiredAttemptId, TestConstants.TestDurationInMinutes + 5);

            // Act
            var expiredIds = timerService.GetExpiredAttemptIds();

            // Assert
            Assert.Contains(expiredAttemptId, expiredIds);
            Assert.DoesNotContain(activeAttemptId, expiredIds);
            Assert.Single(expiredIds);
        }


        [Fact]
        public async Task ExpireTestAsync_ShouldUpdateRepositoryAndRemoveTimer()
        {
            // Arrange
            int attemptId = 100;
            int numberOfCalls = 1;
            timerService.StartTimer(attemptId);

            // Act
            await timerService.ExpireTestAsync(attemptId);

            // Assert
            // 1. Verify the repository was called with the correct mapped object properties
            await testAttemptRepository.Received(numberOfCalls).UpdateAsync(Arg.Is<TestAttempt>(attempt => attempt.Id == attemptId && attempt.Status == TestStatus.COMPLETED.ToString()));

            // 2. Verify that it was removed from the internal dictionary (so checking expiration now returns false)
            bool isStillTracking = timerService.CheckExpiration(attemptId);
            Assert.False(isStillTracking);
        }

        private void BackdateTimer(int attemptId, int minutesToSubtract)
        {
            var field = typeof(TimerService).GetField("Timers", BindingFlags.Static | BindingFlags.NonPublic);
            if (field?.GetValue(null) is ConcurrentDictionary<int, DateTime> timers)
            {
                timers[attemptId] = DateTime.UtcNow.AddMinutes(-minutesToSubtract);
            }
        }
    }
}
