using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SlotsController : ControllerBase
{
    private readonly ISlotService slotService;

    public SlotsController(ISlotService slotService)
    {
        this.slotService = slotService;
    }

    [HttpGet("{slotId}")]
    public async Task<ActionResult<SlotDto>> GetById(int slotId)
    {
        try
        {
            Slot slot = await this.slotService.GetSlotByIdAsync(slotId);
            return Ok(slot.ToDto());
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
    }

    [HttpGet("recruiter/{recruiterId}")]
    public async Task<ActionResult<List<SlotDto>>> GetAllByRecruiter(int recruiterId)
    {
        List<Slot> slots = await this.slotService.GetAllSlotsAsync(recruiterId);
        return Ok(slots.Select(slot => slot.ToDto()).ToList());
    }

    [HttpGet("candidate/{candidateId}")]
    public async Task<ActionResult<List<SlotDto>>> GetByCandidate(int candidateId)
    {
        List<SlotDto> slots = await this.slotService.GetSlotsByCandidateAsync(candidateId);
        return Ok(slots);
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<SlotDto>>> GetAvailableByDate([FromQuery] DateTime date)
    {
        List<SlotDto> slots = await this.slotService.GetAvailableSlotsForDateAsync(date);
        return Ok(slots);
    }

    [HttpGet("recruiter/{recruiterId}/date")]
    public async Task<ActionResult<List<SlotDto>>> GetByRecruiter(int recruiterId, [FromQuery] DateTime date)
    {
        List<Slot> slots = await this.slotService.GetSlotsAsync(recruiterId, date);
        return Ok(slots.Select(slot => slot.ToDto()).ToList());
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<List<SlotDto>>> GetByCompany(int companyId, [FromQuery] DateTime? date)
    {
        DateTime slotDate = date ?? DateTime.Today;
        List<Slot> slots = await this.slotService.GetAvailableSlotsByCompanyAsync(companyId, slotDate);
        return Ok(slots.Select(slot => slot.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<SlotDto>> Create([FromBody] SlotDto slotDto)
    {
        try
        {
            Slot createdSlot = await this.slotService.AddSlotAsync(slotDto.ToEntity());
            return Ok(createdSlot);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{slotId}")]
    public async Task<ActionResult<SlotDto>> Update(int slotId, [FromBody] SlotDto slotDto)
    {
        try
        {
            Slot updatedSlot = await this.slotService.UpdateSlotAsync(slotId, slotDto.ToEntity());
            return Ok(updatedSlot);
        }
        catch (Exception exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpDelete("{slotId}")]
    public async Task<ActionResult> Delete(int slotId)
    {
        try
        {
            bool deleted = await this.slotService.DeleteSlotAsync(slotId);

            if (deleted)
            {
                return Ok(new { message = "Slot was deleted successfully" });
            }

            return BadRequest();
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
    }

    [HttpGet("recruiter/{recruiterId}/visible")]
    public async Task<ActionResult<List<SlotDto>>> GetVisibleSlots(int recruiterId, [FromQuery] DateTime date)
    {
        List<SlotDto> slots = await this.slotService.LoadRecruiterVisibleSlotsAsync(recruiterId, date);
        return Ok(slots);
    }

    [HttpPost("recruiter/create")]
    public async Task<ActionResult> CreateRecruiterSlot([FromBody] CreateSlotDto createSlotDto)
    {
        await this.slotService.CreateRecruiterSlotAsync(createSlotDto.BaseSlot, createSlotDto.Duration);
        return Ok();
    }

    [HttpPut("recruiter/update")]
    public async Task<ActionResult> UpdateRecruiterSlot([FromBody] UpdateSlotDto updateSlotDto)
    {
        try
        {
            await this.slotService.UpdateRecruiterSlotAsync(updateSlotDto.InitialSlot, updateSlotDto.StartTime, updateSlotDto.Duration);
            return Ok();
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}