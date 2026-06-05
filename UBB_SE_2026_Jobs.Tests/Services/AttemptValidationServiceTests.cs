using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services
{
    public class AttemptValidationServiceTests
    {
        private readonly ITestAttemptRepository attemptRepository;
        private readonly AttemptValidationService service;

        public AttemptValidationServiceTests()
        {
            this.attemptRepository = Substitute.For<ITestAttemptRepository>();
            this.service = new AttemptValidationService(this.attemptRepository);
        }

        [Fact]
        public async Task CanStartTestAsync_NonExistingAttempt_ReturnsTrue()
        {
            int userId = 1;
            int testId = 10;
            this.attemptRepository
                .FindByUserAndTestAsync(userId, testId)
                .Returns((TestAttempt?)null);

            var result = await this.service.CanStartTestAsync(userId, testId);


            Assert.True(result);
            await this.attemptRepository
                .Received(1)
                .FindByUserAndTestAsync(userId, testId);
        }

        [Fact]
        public async Task CanStartTestAsync_ExistingAttempt_ReturnsFalse()
        {
            int userId = 1;
            int testId = 10;
            this.attemptRepository
                .FindByUserAndTestAsync(userId, testId)
                .Returns((TestAttempt) new TestAttempt());

            var result = await this.service.CanStartTestAsync(userId, testId);

            Assert.False(result);
            await this.attemptRepository
                .Received(1)
                .FindByUserAndTestAsync(userId, testId);
        }

        [Fact]
        public async Task CheckExistingAttemptsAsync_NonExistingAttempt_NoException()
        {
            int userId = 1;
            int testId = 10;
            this.attemptRepository
                .FindByUserAndTestAsync(userId, testId)
                .Returns((TestAttempt?)null);

             var exception = await Record.ExceptionAsync(()=>this.service.CheckExistingAttemptsAsync(userId, testId));


            Assert.Null(exception);
            await this.attemptRepository
                .Received(1)
                .FindByUserAndTestAsync(userId, testId);
        }

        [Fact]
        public async Task CheckExistingAttemptsAsync_ExistingAttempt_ThrowsException()
        {
            int userId = 1;
            int testId = 10;
            this.attemptRepository
                .FindByUserAndTestAsync(userId, testId)
                .Returns((TestAttempt)new TestAttempt());

            InvalidOperationException exception =await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.CheckExistingAttemptsAsync(userId, testId));
            Assert.Contains("has an active in-progress attempt for test", exception.Message);


            await this.attemptRepository
                .Received(1)
                .FindByUserAndTestAsync(userId, testId);
        }
    }
}
