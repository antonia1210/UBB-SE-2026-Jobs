using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Repositories.Matches;
using UBB_SE_2026_Jobs.Library.Services;
using Xunit;

namespace UBB_SE_2026_Jobs.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly ISlotRepository slotRepository;
        private readonly IInterviewSessionRepository interviewSessionRepository;
        private readonly IMatchRepository matchRepository;
        private readonly BookingService service;

        public BookingServiceTests()
        {
            slotRepository = Substitute.For<ISlotRepository>();
            interviewSessionRepository = Substitute.For<IInterviewSessionRepository>();
            matchRepository = Substitute.For<IMatchRepository>();
            service = new BookingService(slotRepository, interviewSessionRepository, matchRepository);
        }

        [Fact]
        public async Task ConfirmBookingAsync_SlotDoesNotExist_ThrowsKeyNotFoundException()
        {
            int slotId = 1;
            slotRepository.GetByIdAsync(slotId).Returns((Slot?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.ConfirmBookingAsync(slotId, 10, 100));
        }

        [Fact]
        public async Task ConfirmBookingAsync_SlotIsNotFree_ThrowsInvalidOperationException()
        {
            var slot = new Slot { Id = 1, Status = SlotStatus.Occupied };
            slotRepository.GetByIdAsync(1).Returns(slot);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.ConfirmBookingAsync(1, 10, 100));
        }

        [Fact]
        public async Task ConfirmBookingAsync_SlotIsFree_UpdatesSlotAndCreatesSession()
        {
            int slotId = 1;
            int candidateId = 10;
            int jobId = 100;
            var slot = new Slot
            {
                Id = slotId,
                Status = SlotStatus.Free,
                RecruiterId = 5,
                StartTime = DateTime.UtcNow
            };
            slotRepository.GetByIdAsync(slotId).Returns(slot);

            await service.ConfirmBookingAsync(slotId, candidateId, jobId);

            Assert.Equal(SlotStatus.Occupied, slot.Status);
            Assert.Equal(candidateId, slot.CandidateId);
            Assert.Equal(string.Empty, slot.InterviewType);

            await slotRepository.Received(1).UpdateAsync(slot);
            interviewSessionRepository.Received(1).Add(Arg.Is<InterviewSession>(interviewSession =>
                interviewSession.PositionId == jobId &&
                interviewSession.ExternalUserId == candidateId &&
                interviewSession.InterviewerId == slot.RecruiterId &&
                interviewSession.DateStart == slot.StartTime.ToUniversalTime() &&
                interviewSession.Video == string.Empty &&
                interviewSession.Status == "Scheduled" &&
                interviewSession.Score == 0
            ));
        }
    }
}
