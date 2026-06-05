using System;
using Xunit;
using UBB_SE_2026_Jobs.Library.Validators;

namespace UBB_SE_2026_Jobs.Tests.Validators
{
    public class EventValidatorTests
    {
        private const string ValidTitle = "Valid Event Title";
        private const string ValidDescription = "A very nice event.";
        private const string ValidLocation = "Cluj-Napoca";


        private const int TitleLengthExceeded = 201;
        private const int DescriptionLengthExceeded = 2001;
        private const int LocationLengthExceeded = 301;
        private const string WhitespaceString = "   ";
        private const char FillerChar = 'a';

        private readonly IEventValidator eventValidator;

        public EventValidatorTests()
        {
            eventValidator = new EventValidator();
        }

       
        [Fact]
        public void ValidateEventTitle_ValidTitle_ReturnsTrue()
        {
            var result = eventValidator.ValidateEventTitle(ValidTitle);
            Assert.True(result);
        }

        [Fact]
        public void ValidateEventTitle_WhitespaceTitle_ThrowsException()
        {
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventTitle(WhitespaceString));
        }

        [Fact]
        public void ValidateEventTitle_TitleExceedsMaxLength_ThrowsException()
        {
            var longTitle = new string(FillerChar, TitleLengthExceeded);
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventTitle(longTitle));
        }


        [Fact]
        public void ValidateEventDescription_ValidDescription_ReturnsTrue()
        {
            var result = eventValidator.ValidateEventDescription(ValidDescription);
            Assert.True(result);
        }


        [Fact]
        public void ValidateEventDescription_EmptyDescription_ReturnsTrue()
        {
            var result = eventValidator.ValidateEventDescription("");
            Assert.True(result);
        }

        [Fact]
        public void ValidateEventDescription_DescriptionExceedsMaxLength_ThrowsException()
        {
            var longDescription = new string(FillerChar, DescriptionLengthExceeded );
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventDescription(longDescription));
        }


        [Fact]
        public void ValidateEventLocation_ValidLocation_ReturnsTrue()
        {
            var result = eventValidator.ValidateEventLocation(ValidLocation);
            Assert.True(result);
        }

        [Fact]
        public void ValidateEventLocation_WhitespaceLocation_ThrowsException()
        {
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventLocation(WhitespaceString));
            
        }

        [Fact]
        public void ValidateEventLocation_LocationExceedsMaxLength_ThrowsException()
        {
            var longLocation = new string(FillerChar, LocationLengthExceeded);
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventLocation(longLocation));
          
        }


        [Fact]
        public void ValidateEventStartDate_NullStartDate_ThrowsException()
        {
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventStartDate(null));
        }

        [Fact]
        public void ValidateEventStartDate_StartDateInPast_ThrowsException()
        {
            var oneDayAgo = -1;
            var pastDate = DateTimeOffset.Now.AddDays(oneDayAgo);
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventStartDate(pastDate));
        }

        [Fact]
        public void ValidateEventStartDate_StartDateInFuture_ReturnsTrue()
        {
            var oneDayInFuture = 1;
            var futureDate = DateTimeOffset.Now.AddDays(oneDayInFuture);
            var result = eventValidator.ValidateEventStartDate(futureDate);
            Assert.True(result);
        }

        // ValidateEventEndDate

        [Fact]
        public void ValidateEventEndDate_NullEndDate_ThrowsException()
        {
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventEndDate(null));
         
        }

        [Fact]
        public void ValidateEventEndDate_EndDateInPast_ThrowsException()
        {
            var oneDayAgo = -1;
            var pastDate = DateTimeOffset.Now.AddDays(oneDayAgo);
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventEndDate(pastDate));
           
        }

        [Fact]
        public void ValidateEventEndDate_EndDateInFuture_ReturnsTrue()
        {
            var oneDayInFuture = 1;
            var futureDate = DateTimeOffset.Now.AddDays(oneDayInFuture);
            var result = eventValidator.ValidateEventEndDate(futureDate);
            Assert.True(result);
        }


        [Fact]
        public void ValidateEventDatesChronologically_StartDateAfterEndDate_ThrowsException()
        {
            var oneDayInFuture = 1;
            var twoDaysInFuture = 2;
            var startDate = DateTimeOffset.Now.AddDays(twoDaysInFuture);
            var endDate = DateTimeOffset.Now.AddDays(oneDayInFuture);
            var exception = Assert.Throws<Exception>(() => eventValidator.ValidateEventDatesChronologically(startDate, endDate));
           
        }

        [Fact]
        public void ValidateEventDatesChronologically_StartDateBeforeEndDate_ReturnsTrue()
        {
            var oneDayInFuture = 1;
            var twoDaysInFuture = 2;
            var startDate = DateTimeOffset.Now.AddDays(oneDayInFuture);
            var endDate = DateTimeOffset.Now.AddDays(twoDaysInFuture);
            var result = eventValidator.ValidateEventDatesChronologically(startDate, endDate);
            Assert.True(result);
        }

    }
}