namespace UBB_SE_2026_Jobs.Library.Validators
{
    public interface IPaymentValidator
    {
        string ValidatePaymentDetails(string cardHolderName, string cardNumber, string expirationDate, string cardVerificationValue);
    }
}