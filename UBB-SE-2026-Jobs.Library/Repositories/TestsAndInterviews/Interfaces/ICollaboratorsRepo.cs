namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;

    public interface ICollaboratorsRepo
    {
        void AddCollaboratorToRepo(Event eventOfCollaboration, Company collaboratorToBeAdded, int loggedInUserID);
        List<Company> GetAllCollaborators(int loggedInCompanyId);
    }
}
