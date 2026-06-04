namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Validators
{
    public interface IPaymentValidator
    {
        string ValidatePaymentDetails(string cardHolderName, string cardNumber, string expirationDate, string cardVerificationValue);
    }
}