using UBB_SE_2026_Jobs.Library.Domain.Enums;

namespace UBB_SE_2026_Jobs.Library.Domain;

public static class MatchStatusTransitions
{
    public static bool IsDecisionTransitionAllowed(MatchStatus current, MatchStatus next)
    {
        if (current == MatchStatus.Applied)
            return next is MatchStatus.Accepted or MatchStatus.Rejected or MatchStatus.Advanced;

        if (current == MatchStatus.Advanced)
            return next is MatchStatus.Accepted or MatchStatus.Rejected;

        return false;
    }
}
