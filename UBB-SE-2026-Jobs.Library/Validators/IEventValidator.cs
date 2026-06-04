using System;

namespace UBB_SE_2026_Jobs.Library.Validators
{
    public interface IEventValidator
    {
        bool ValidateEventTitle(string eventTitle);
        bool ValidateEventDescription(string eventDescription);
        bool ValidateEventLocation(string eventLocation);
        bool ValidateEventStartDate(DateTimeOffset? eventStartDate);
        bool ValidateEventEndDate(DateTimeOffset? eventEndDate);
        bool ValidateEventDatesChronologically(DateTimeOffset? eventStartDate, DateTimeOffset? eventEndDate);
    }
}