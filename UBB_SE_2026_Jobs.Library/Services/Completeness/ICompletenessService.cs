using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Services.CompletenessService;

public interface ICompletenessService
{
    int CalculateCompleteness(User? user);
    string GetNextEmptyFieldPrompt(User? user);
}
