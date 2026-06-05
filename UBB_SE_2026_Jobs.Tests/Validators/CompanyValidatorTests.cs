using System;
using System.ComponentModel.DataAnnotations;
using UBB_SE_2026_Jobs.Library.Validators;
using Xunit;

namespace UBB_SE_2026_Jobs.Tests.Validators
{
    public class CompanyValidatorTests
    {
        private const char FillerChar = 'a';
        private const int NameLengthExceeded = 201;
        private const int AboutUsLengthExceeded = 2001;
        private const int LocationLengthExceeded = 301;
        private const int EmailLengthExceeded = 101;

        private const string ValidName = "Google";
        private const string ValidAboutUs = "We build software.";
        private const string ValidLocation = "Bucharest";
        private const string InvalidEmailNoAt = "invalidemail";
        private const string EmailDomain = "@gmail.com";
        private const string ValidEmail = "test@gmail.com";
        private const string InvalidImageExtension = "image.txt";
        private const string ValidImagePng = "image.png";
        private const string InvalidLogoExtension = "logo.txt";
        private const string ValidLogoJpg = "logo.jpg";
        private const string ValidBase64Png = "data:image/png;base64,AAA";
        private const string InvalidBase64Mime = "data:image/webp;base64,abc123";
        private const string WhitespaceString = "   ";

        private readonly ICompanyValidator companyValidator;
        public CompanyValidatorTests()
        {
            companyValidator = new CompanyValidator();
        }

        [Fact]
        public void ValidateName_ValidName_ReturnsTrue()
        {
            bool result = companyValidator.ValidateName(ValidName);
            Assert.True(result);
        }


        [Fact]
        public void ValidateName_EmptyName_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateName(""));
        }

        [Fact]
        public void ValidateName_ExceedsMaxLength_ThrowsException()
        {
            string nameTooLong = new string(FillerChar, NameLengthExceeded);
            
            Assert.Throws<Exception>(() => companyValidator.ValidateName(nameTooLong));
        }

        [Fact]
        public void ValidateAboutUs_ValidDescription_ReturnsTrue()
        {
            bool result = companyValidator.ValidateAboutUs(ValidAboutUs);
            Assert.True(result);
        }

        [Fact]
        public void ValidateAboutUs_EmptyDescription_ReturnsTrue()
        {
            bool result = companyValidator.ValidateAboutUs("");
            Assert.True(result);
        }

        [Fact]
        public void ValidateAboutUs_ExceedsMaxLength_ThrowsException()
        {
            string descriptionTooLong = new string(FillerChar, AboutUsLengthExceeded);
            Assert.Throws<Exception>(() => companyValidator.ValidateAboutUs(descriptionTooLong));
        }


        [Fact]
        public void ValidateLocation_ValidLocation_ReturnsTrue()
        {
            bool result = companyValidator.ValidateLocation(ValidLocation);
            Assert.True(result);
        }

        [Fact]
        public void ValidateLocation_EmptyLocation_ReturnsTrue()
        {
            bool result = companyValidator.ValidateLocation("");
            Assert.True(result);
        }


        [Fact]
        public void ValidateLocation_ExceedsMaxLength_ThrowsException()
        {
            string locationTooLong = new string(FillerChar,LocationLengthExceeded);
            Assert.Throws<Exception>(() => companyValidator.ValidateLocation(locationTooLong));
        }


        [Fact]
        public void ValidateEmail_ValidEmail_ReturnsTrue()
        {
            bool result = companyValidator.ValidateEmail(ValidEmail);
            Assert.True(result);
        }

        [Fact]
        public void ValidateEmail_EmptyEmail_ReturnsTrue()
        {
            bool result = companyValidator.ValidateEmail("");
            Assert.True(result);
        }

        [Fact]
        public void ValidateEmail_MissingAtSign_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateEmail(InvalidEmailNoAt));
        }

        [Fact]
        public void ValidateEmail_ExceedsMaxLength_ThrowsException()
        {
            string email = new string(FillerChar, EmailLengthExceeded) + EmailDomain;
            Assert.Throws<Exception>(() => companyValidator.ValidateEmail(email));
        }

        [Fact]
        public void ValidateProfilePicture_ValidPngExtension_ReturnsTrue()
        {
            bool result = companyValidator.ValidateProfilePicture(ValidImagePng);
            Assert.True(result);
        }


        [Fact]
        public void ValidateProfilePicture_NullPath_ReturnsTrue()
        {
            bool result = companyValidator.ValidateProfilePicture(null);
            Assert.True(result);
        }

        [Fact]
        public void ValidateProfilePicture_InvalidExtension_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateProfilePicture(InvalidImageExtension));
        }


        [Fact]
        public void ValidateLogo_WhitespaceLogo_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateLogo(WhitespaceString));
        }

        [Fact]
        public void ValidateLogo_ValidJpgExtension_ReturnsTrue()
        {
            bool result = companyValidator.ValidateLogo(ValidLogoJpg);
            Assert.True(result);
        }


        [Fact]
        public void ValidateLogo_InvalidExtension_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateLogo(InvalidLogoExtension));
        }
        
        [Fact]
        public void ValidateLogo_Base64Png_ReturnsTrue()
        {
            bool result = companyValidator.ValidateLogo(ValidBase64Png);
            Assert.True(result);
        }

        [Fact]
        public void ValidateLogo_Base64InvalidMime_ThrowsException()
        {
            Assert.Throws<Exception>(() => companyValidator.ValidateLogo(InvalidBase64Mime));
        }
    }
}