using System.Collections.Generic;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;

namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces
{
    public interface IProfileCompletionCalculator
    {
        (int percentage, List<string> remainingTasks) Calculate(Company company);

        (List<string> skillNames, List<int> percents) GetSkillsTop3(int companyId);

        string ApplicantsMessage(int companyId);
    }
}