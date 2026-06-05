namespace UBB_SE_2026_Jobs.Library.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain;

    public interface ICollaboratorsRepo
    {
        void AddCollaboratorToRepo(Event eventOfCollaboration, Company collaboratorToBeAdded, int loggedInUserID);
        List<Company> GetAllCollaborators(int loggedInCompanyId);

        /// <summary>
        /// Retrieves all collaborators associated with the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <returns>A list of companies that are collaborators of the specified event.</returns>
        List<Company> GetEventCollaborators(int eventId);
    }
}
