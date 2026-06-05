using System;
using System.Collections.Generic;
using Xunit;
using UBB_SE_2026_Jobs.Library.Validators;

namespace UBB_SE_2026_Jobs.Tests.Validators
{
    public class PaymentValidatorTests
    {
        

        private const string ValidCardHolderName = "John Doe";
        private const string ValidCardNumber = "123456789012345";
        private const string ValidExpirationDate = "12/99";
        private const string ValidCvv = "123";
        private const string ExpirationDateNonNumericMonth = "AB/99";
        private const string ExpirationDateNonNumericYear = "12/YY";
        private const string ExpirationDateMonthZero = "00/99";
        private const string ExpirationDateMonthThirteen = "13/99";
        private const string ExpirationDateExpired = "01/00";

        private const string ExpirationDateWithoutSeparator = "1299";


        private const string ErrorCardHolderNameRequired = "Card Holder Name is required.";
        private const string ErrorCardNumberInvalid = "Please enter a valid Card Number.";
        private const string ErrorExpirationDateFormat = "Expiration Date must be in MM/YY format.";
        private const string ErrorCvvInvalid = "Please enter a valid CVV.";
        private const string ErrorExpirationDateInvalidNumbers = "Expiration Date must contain valid numbers (MM/YY).";
        private const string ErrorExpirationMonthInvalid = "Invalid expiration month. Must be between 01 and 12.";
        private const string ErrorCardExpired = "This card has expired. Please use a valid card.";
        private const string NoError = "";

        private const int CardNumberValidLength = 15;
        private const int CardVerificationValueValidLength = 3;
        private const char FillerCardNumber = '1';

        private readonly IPaymentValidator paymentValidator;
        public PaymentValidatorTests()
        {
            paymentValidator = new PaymentValidator();
        }


        [Fact]
        public void ValidatePaymentDetails_NullCardHolderName_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(null, ValidCardNumber, ValidExpirationDate, ValidCvv);
            Assert.Equal(ErrorCardHolderNameRequired, result);
        }

        [Fact]
        public void ValidatePaymentDetails_WhitespaceCardHolderName_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails("   ", ValidCardNumber, ValidExpirationDate, ValidCvv);
            Assert.Equal(ErrorCardHolderNameRequired, result);
        }

        [Fact]
        public void ValidatePaymentDetails_CardNumberTooShort_ReturnsError()
        {
            var shortCardNumber = new string(FillerCardNumber, CardNumberValidLength - 1);
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, shortCardNumber, ValidExpirationDate, ValidCvv);
            Assert.Equal(ErrorCardNumberInvalid, result);
        }

        [Fact]
        public void ValidatePaymentDetails_NullExpirationDate_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, null, ValidCvv);
            Assert.Equal(ErrorExpirationDateFormat, result);
        }

        [Fact]
        public void ValidatePaymentDetails_ExpirationDateWithoutSeparator_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateWithoutSeparator, ValidCvv);
            Assert.Equal(ErrorExpirationDateFormat, result);
        }


        [Fact]
        public void ValidatePaymentDetails_NullCvv_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ValidExpirationDate, null);
            Assert.Equal(ErrorCvvInvalid, result);
        }

        [Fact]
        public void ValidatePaymentDetails_CvvTooShort_ReturnsError()
        {
            var shortCvv = new string(FillerCardNumber, CardVerificationValueValidLength - 1);
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ValidExpirationDate, shortCvv);
            Assert.Equal(ErrorCvvInvalid, result);
        }


        [Fact]
        public void ValidatePaymentDetails_ExpirationDateNonNumericMonth_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateNonNumericMonth, ValidCvv);
            Assert.Equal(ErrorExpirationDateInvalidNumbers, result);
        }

        [Fact]
        public void ValidatePaymentDetails_ExpirationDateNonNumericYear_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateNonNumericYear, ValidCvv);
            Assert.Equal(ErrorExpirationDateInvalidNumbers, result);
        }

        [Fact]
        public void ValidatePaymentDetails_ExpirationMonthZero_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateMonthZero, ValidCvv);
            Assert.Equal(ErrorExpirationMonthInvalid, result);
        }

        [Fact]
        public void ValidatePaymentDetails_ExpirationMonthThirteen_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateMonthThirteen, ValidCvv);
            Assert.Equal(ErrorExpirationMonthInvalid, result);
        }

        [Fact]
        public void ValidatePaymentDetails_ExpiredCard_ReturnsError()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ExpirationDateExpired, ValidCvv);
            Assert.Equal(ErrorCardExpired, result);
        }

        [Fact]
        public void ValidatePaymentDetails_AllValidDetails_ReturnsEmptyString()
        {
            var result = paymentValidator.ValidatePaymentDetails(ValidCardHolderName, ValidCardNumber, ValidExpirationDate, ValidCvv);
            Assert.Equal(string.Empty, result);
        }
    }
}