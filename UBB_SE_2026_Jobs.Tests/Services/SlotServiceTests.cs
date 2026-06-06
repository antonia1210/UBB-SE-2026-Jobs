using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class SlotServiceTests
{
    private readonly ISlotRepository slotRepository = Substitute.For<ISlotRepository>();
    private readonly SlotService slotService;

    private const int WorkdayStartHour = 8;
    private const int WorkdayEndHour = 18;
    private const int DefaultSlotDurationMinutes = 30;
    private const int WorkdayTotalSlots = (WorkdayEndHour - WorkdayStartHour) * 60 / DefaultSlotDurationMinutes;

    public SlotServiceTests()
    {
        slotService = new SlotService(slotRepository);
    }

    // Helpers

    private static DateTime WorkdayDate() =>
        new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    private static DateTime WorkdayStart() =>
        WorkdayDate().AddHours(WorkdayStartHour);

    private static Slot FreeSlot(int id, DateTime start, int durationMinutes = DefaultSlotDurationMinutes) => new()
    {
        Id = id,
        StartTime = start,
        EndTime = start.AddMinutes(durationMinutes),
        Duration = durationMinutes,
        Status = SlotStatus.Free,
    };

    private static Slot OccupiedSlot(int id, DateTime start, int durationMinutes = DefaultSlotDurationMinutes) => new()
    {
        Id = id,
        StartTime = start,
        EndTime = start.AddMinutes(durationMinutes),
        Duration = durationMinutes,
        Status = SlotStatus.Occupied,
    };

    private static SlotDto SlotDtoFor(int slotId, int recruiterId, int companyId, DateTime startTime) => new()
    {
        Id = slotId,
        RecruiterId = recruiterId,
        CompanyId = companyId,
        StartTime = startTime,
    };

    // GetSlotByIdAsync

    [Fact]
    public async Task GetSlotByIdAsync_SlotExists_ReturnsSlot()
    {
        var slot = FreeSlot(id: 1, start: WorkdayStart());
        slotRepository.GetByIdAsync(1).Returns(slot);

        var result = await slotService.GetSlotByIdAsync(id: 1);

        Assert.Same(slot, result);
    }

    [Fact]
    public async Task GetSlotByIdAsync_SlotNotFound_ThrowsKeyNotFoundException()
    {
        slotRepository.GetByIdAsync(999).Returns((Slot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => slotService.GetSlotByIdAsync(id: 999));
    }

    // DeleteSlotAsync

    [Fact]
    public async Task DeleteSlotAsync_SlotExists_ReturnsTrue()
    {
        var slot = FreeSlot(id: 1, start: WorkdayStart());
        slotRepository.GetByIdAsync(1).Returns(slot);

        var result = await slotService.DeleteSlotAsync(id: 1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteSlotAsync_SlotExists_CallsRepositoryDelete()
    {
        var slot = FreeSlot(id: 1, start: WorkdayStart());
        slotRepository.GetByIdAsync(1).Returns(slot);

        await slotService.DeleteSlotAsync(id: 1);

        await slotRepository.Received(1).DeleteAsync(1);
    }

    [Fact]
    public async Task DeleteSlotAsync_SlotNotFound_ThrowsKeyNotFoundException()
    {
        slotRepository.GetByIdAsync(999).Returns((Slot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => slotService.DeleteSlotAsync(id: 999));
    }

    [Fact]
    public async Task DeleteSlotAsync_SlotNotFound_DoesNotCallRepositoryDelete()
    {
        slotRepository.GetByIdAsync(999).Returns((Slot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => slotService.DeleteSlotAsync(id: 999));

        await slotRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task UpdateSlotAsync_SlotExists_ReturnsSlotWithRouteId()
    {
        const int routeId = 99;
        var incomingSlot = FreeSlot(id: 1, start: WorkdayStart());
        slotRepository.GetByIdAsync(incomingSlot.Id).Returns(incomingSlot);

        var result = await slotService.UpdateSlotAsync(routeId, incomingSlot);

        // The slotService assigns the route id to the slot after updating — document this behavior.
        Assert.Equal(routeId, result.Id);
    }

    [Fact]
    public async Task UpdateSlotAsync_SlotExists_CallsRepositoryUpdate()
    {
        var incomingSlot = FreeSlot(id: 1, start: WorkdayStart());
        slotRepository.GetByIdAsync(incomingSlot.Id).Returns(incomingSlot);

        await slotService.UpdateSlotAsync(id: 1, incomingSlot);

        await slotRepository.Received(1).UpdateAsync(incomingSlot);
    }

    [Fact]
    public async Task UpdateSlotAsync_SlotNotFound_ThrowsKeyNotFoundException()
    {
        var incomingSlot = FreeSlot(id: 999, start: WorkdayStart());
        slotRepository.GetByIdAsync(incomingSlot.Id).Returns((Slot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => slotService.UpdateSlotAsync(id: 999, incomingSlot));
    }

    [Fact]
    public async Task UpdateSlotAsync_SlotNotFound_DoesNotCallRepositoryUpdate()
    {
        var incomingSlot = FreeSlot(id: 999, start: WorkdayStart());
        slotRepository.GetByIdAsync(incomingSlot.Id).Returns((Slot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => slotService.UpdateSlotAsync(id: 999, incomingSlot));

        await slotRepository.DidNotReceive().UpdateAsync(Arg.Any<Slot>());
    }

    // UpdateRecruiterSlotAsync

    [Fact]
    public async Task UpdateRecruiterSlotAsync_StartTimeBeforeWorkdayStart_ThrowsArgumentException()
    {
        var startTimeBeforeWorkday = WorkdayDate().AddHours(WorkdayStartHour - 1);
        var slotDto = SlotDtoFor(slotId: 1, recruiterId: 1, companyId: 1, startTime: startTimeBeforeWorkday);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            slotService.UpdateRecruiterSlotAsync(slotDto, startTimeBeforeWorkday, DefaultSlotDurationMinutes));
    }

    [Fact]
    public async Task UpdateRecruiterSlotAsync_StartTimeAfterWorkdayEnd_ThrowsArgumentException()
    {
        var startTimeAfterWorkday = WorkdayDate().AddHours(WorkdayEndHour + 1);
        var slotDto = SlotDtoFor(slotId: 1, recruiterId: 1, companyId: 1, startTime: startTimeAfterWorkday);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            slotService.UpdateRecruiterSlotAsync(slotDto, startTimeAfterWorkday, DefaultSlotDurationMinutes));
    }

    [Fact]
    public async Task UpdateRecruiterSlotAsync_StartTimeWithinWorkday_CallsRepositoryUpdate()
    {
        var validStartTime = WorkdayDate().AddHours(WorkdayStartHour);
        var slotDto = SlotDtoFor(slotId: 1, recruiterId: 1, companyId: 1, startTime: validStartTime);

        await slotService.UpdateRecruiterSlotAsync(slotDto, validStartTime, DefaultSlotDurationMinutes);

        await slotRepository.Received(1).UpdateAsync(Arg.Is<Slot>(slot =>
            slot.Id == slotDto.Id &&
            slot.StartTime == validStartTime &&
            slot.EndTime == validStartTime.AddMinutes(DefaultSlotDurationMinutes) &&
            slot.Duration == DefaultSlotDurationMinutes));
    }

    [Fact]
    public async Task UpdateRecruiterSlotAsync_ValidStartTime_SetsEndTimeByAddingDuration()
    {
        const int customDurationMinutes = 60;
        var validStartTime = WorkdayDate().AddHours(WorkdayStartHour);
        var slotDto = SlotDtoFor(slotId: 1, recruiterId: 1, companyId: 1, startTime: validStartTime);

        await slotService.UpdateRecruiterSlotAsync(slotDto, validStartTime, customDurationMinutes);

        await slotRepository.Received(1).UpdateAsync(Arg.Is<Slot>(slot =>
            slot.EndTime == validStartTime.AddMinutes(customDurationMinutes)));
    }

    // GetAvailableSlotsByCompanyAsync

    [Fact]
    public async Task GetAvailableSlotsByCompanyAsync_SlotsFromMultipleCompanies_ReturnsOnlyMatchingCompany()
    {
        const int targetCompanyId = 1;
        const int otherCompanyId = 2;
        var date = WorkdayDate();

        var slotsFromRepository = new List<Slot>
        {
            new() { Id = 1, RecruiterCompanyId = targetCompanyId, StartTime = WorkdayStart() },
            new() { Id = 2, RecruiterCompanyId = otherCompanyId,  StartTime = WorkdayStart() },
            new() { Id = 3, RecruiterCompanyId = targetCompanyId, StartTime = WorkdayStart().AddMinutes(DefaultSlotDurationMinutes) },
        };
        slotRepository.GetAvailableByDateAsync(date).Returns(slotsFromRepository);

        var result = await slotService.GetAvailableSlotsByCompanyAsync(targetCompanyId, date);

        Assert.Equal(2, result.Count);
        Assert.All(result, slot => Assert.Equal(targetCompanyId, slot.RecruiterCompanyId));
    }

    [Fact]
    public async Task GetAvailableSlotsByCompanyAsync_NoSlotsForCompany_ReturnsEmptyList()
    {
        const int targetCompanyId = 1;
        var date = WorkdayDate();
        slotRepository.GetAvailableByDateAsync(date).Returns(new List<Slot>());

        var result = await slotService.GetAvailableSlotsByCompanyAsync(targetCompanyId, date);

        Assert.Empty(result);
    }

    // LoadRecruiterVisibleSlotsAsync

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_NoExistingSlots_FillsEntireWorkdayWithFreeSlots()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.Equal(WorkdayTotalSlots, result.Count);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_NoExistingSlots_AllGeneratedSlotsAreFree()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.All(result, slot => Assert.Equal((int)SlotStatus.Free, slot.Status));
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_NoExistingSlots_FirstSlotStartsAtWorkdayStart()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.Equal(WorkdayStart(), result.First().StartTime);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_NoExistingSlots_LastSlotEndsAtWorkdayEnd()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.Equal(WorkdayDate().AddHours(WorkdayEndHour), result.Last().EndTime);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_OneExistingSlotAtStart_IncludesItInsteadOfGeneratedSlot()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        var existingSlot = OccupiedSlot(id: 1, start: WorkdayStart());
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot> { existingSlot });

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.Equal(WorkdayTotalSlots, result.Count);
        Assert.Equal(existingSlot.Id, result.First().Id);
        Assert.Equal((int)existingSlot.Status, result.First().Status);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_ExistingSlotWithLongerDuration_AdvancesTimeBySlotDuration()
    {
        const int recruiterId = 1;
        const int longerSlotDurationMinutes = 60;
        var date = WorkdayDate();
        var longSlot = OccupiedSlot(id: 1, start: WorkdayStart(), durationMinutes: longerSlotDurationMinutes);
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot> { longSlot });

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        // After consuming the 60-minute slot, remaining workday fills with 30-minute slots
        int remainingMinutes = (WorkdayEndHour - WorkdayStartHour) * 60 - longerSlotDurationMinutes;
        int remainingFreeSlots = remainingMinutes / DefaultSlotDurationMinutes;
        Assert.Equal(1 + remainingFreeSlots, result.Count);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_ExistingSlotInMiddleOfDay_SlotsBothBeforeAndAfterAreGenerated()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        var middayStart = WorkdayStart().AddHours(1);
        var existingSlot = OccupiedSlot(id: 1, start: middayStart);
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot> { existingSlot });

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        // Slots before the existing one, the existing one itself, and slots after
        Assert.Equal(WorkdayTotalSlots, result.Count);
        var existingInResult = result.Single(slot => slot.Id == existingSlot.Id);
        Assert.Equal((int)existingSlot.Status, existingInResult.Status);
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_GeneratedFreeSlots_HaveDefaultDuration()
    {
        const int recruiterId = 1;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.All(result, slot => Assert.Equal(DefaultSlotDurationMinutes, slot.Duration));
    }

    [Fact]
    public async Task LoadRecruiterVisibleSlotsAsync_GeneratedFreeSlots_AssignCorrectRecruiterId()
    {
        const int recruiterId = 42;
        var date = WorkdayDate();
        slotRepository.GetSlotsAsync(recruiterId, date).Returns(new List<Slot>());

        var result = await slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

        Assert.All(result, slot => Assert.Equal(recruiterId, slot.RecruiterId));
    }
}