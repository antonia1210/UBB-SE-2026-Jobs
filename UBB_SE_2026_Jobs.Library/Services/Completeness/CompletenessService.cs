using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;

namespace UBB_SE_2026_Jobs.Library.Services.Completeness;

public class CompletenessService : ICompletenessService
{
    private const int TotalFields = 21;

    private const int FieldIndexFirstName = 0;
    private const int FieldIndexLastName = 1;
    private const int FieldIndexAge = 2;
    private const int FieldIndexGender = 3;
    private const int FieldIndexCountry = 4;
    private const int FieldIndexPhone = 5;
    private const int FieldIndexEmail = 6;
    private const int FieldIndexUniversity = 7;
    private const int FieldIndexGraduationYear = 8;
    private const int FieldIndexGitHub = 9;
    private const int FieldIndexLinkedIn = 10;
    private const int FieldIndexAddress = 11;
    private const int FieldIndexProfilePicture = 12;
    private const int FieldIndexSkills = 13;
    private const int FieldIndexMotivation = 14;
    private const int FieldIndexWorkExperience = 15;
    private const int FieldIndexProjects = 16;
    private const int FieldIndexActivities = 17;
    private const int FieldIndexPreferredRoles = 18;
    private const int FieldIndexWorkMode = 19;
    private const int FieldIndexLocationPreference = 20;

    private const int MinimumAge = 0;
    private const int MinimumGraduationYear = 0;
    private const int MinimumCollectionCount = 0;
    private const int PercentageMultiplier = 100;

    private static readonly string[] Labels =
    {
        "First Name", "Last Name", "Age", "Gender", "Country",
        "Phone Number", "Email", "University", "Graduation Year", "GitHub",
        "LinkedIn", "Address", "Profile Picture", "Skills", "Motivation",
        "Work Experience", "Projects", "Activities", "Preferred Roles",
        "Work Mode", "Location Preference"
    };

    private bool IsFieldFilled(int fieldIndex, User user)
    {
        switch (fieldIndex)
        {
            case FieldIndexFirstName: return !string.IsNullOrWhiteSpace(user.FirstName);
            case FieldIndexLastName: return !string.IsNullOrWhiteSpace(user.LastName);
            case FieldIndexAge: return user.Age > MinimumAge;
            case FieldIndexGender: return !string.IsNullOrWhiteSpace(user.Gender);
            case FieldIndexCountry: return !string.IsNullOrWhiteSpace(user.Country);
            case FieldIndexPhone: return !string.IsNullOrWhiteSpace(user.Phone);
            case FieldIndexEmail: return !string.IsNullOrWhiteSpace(user.Email);
            case FieldIndexUniversity: return !string.IsNullOrWhiteSpace(user.University);
            case FieldIndexGraduationYear: return user.ExpectedGraduationYear > MinimumGraduationYear;
            case FieldIndexGitHub: return !string.IsNullOrWhiteSpace(user.GitHub);
            case FieldIndexLinkedIn: return !string.IsNullOrWhiteSpace(user.LinkedIn);
            case FieldIndexAddress: return !string.IsNullOrWhiteSpace(user.Address);
            case FieldIndexProfilePicture: return !string.IsNullOrWhiteSpace(user.ProfilePicturePath);
            case FieldIndexSkills: return user.Skills != null && user.Skills.Count > MinimumCollectionCount;
            case FieldIndexMotivation: return !string.IsNullOrWhiteSpace(user.Motivation);
            case FieldIndexWorkExperience: return user.WorkExperiences != null && user.WorkExperiences.Count > MinimumCollectionCount;
            case FieldIndexProjects: return user.Projects != null && user.Projects.Count > MinimumCollectionCount;
            case FieldIndexActivities: return user.ExtraCurricularActivities != null && user.ExtraCurricularActivities.Count > MinimumCollectionCount;
            case FieldIndexPreferredRoles: return user.PersonalityResult?.SelectedRole != null;
            case FieldIndexWorkMode: return !string.IsNullOrWhiteSpace(user.WorkModePreference);
            case FieldIndexLocationPreference: return !string.IsNullOrWhiteSpace(user.LocationPreference);
            default: return false;
        }
    }

    private int CalculateCompletenessPercentage(int filledFields)
    {
        return (int)Math.Round((double)filledFields / TotalFields * PercentageMultiplier);
    }

    private int CountFilledFields(User user)
    {
        int filledFields = 0;

        for (int fieldIndex = 0; fieldIndex < TotalFields; fieldIndex++)
        {
            if (IsFieldFilled(fieldIndex, user))
            {
                filledFields++;
            }
        }

        return filledFields;
    }

    public int CalculateCompleteness(User? user)
    {
        if (user == null)
        {
            return 0;
        }
        int filledFields = CountFilledFields(user);
        return CalculateCompletenessPercentage(filledFields);
    }

    public string GetNextEmptyFieldPrompt(User? user)
    {
        if (user == null)
        {
            return string.Empty;
        }

        int filledFields = CountFilledFields(user);

        for (int fieldIndex = 0; fieldIndex < TotalFields; fieldIndex++)
        {
            if (!IsFieldFilled(fieldIndex, user))
            {
                int nextPercentage = CalculateCompletenessPercentage(filledFields + 1);
                return $"Add your {Labels[fieldIndex]} to reach {nextPercentage}% completeness!";
            }
        }

        return "Your profile is 100% complete!";
    }
}