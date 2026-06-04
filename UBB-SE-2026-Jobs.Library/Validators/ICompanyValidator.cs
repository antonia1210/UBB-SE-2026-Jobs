namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Validators
{
    public interface ICompanyValidator
    {
        bool ValidateName(string companyName);
        bool ValidateAboutUs(string aboutUsDescription);
        bool ValidateLocation(string companyLocation);
        bool ValidateEmail(string companyEmail);
        bool ValidateProfilePicture(string profilePicturePath);
        bool ValidateLogo(string logoPath);
    }
}