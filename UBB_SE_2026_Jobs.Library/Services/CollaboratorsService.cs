namespace UBB_SE_2026_Jobs.Library.Services
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing collaborators.
    /// </summary>
    public class CollaboratorsService : ICollaboratorsService
    {
        private readonly ICollaboratorsRepo _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollaboratorsService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access collaborator data. Cannot be null.</param>
        public CollaboratorsService(ICollaboratorsRepo repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Adds a collaborator to the specified event.
        /// </summary>
        /// <param name="eventOfCollaboration">The event to which the collaborator will be added.</param>
        /// <param name="collaboratorToBeAdded">The company to be added as a collaborator.</param>
        /// <param name="loggedInUserID">The unique identifier of the logged in user.</param>
        public void AddCollaboratorToRepo(Event eventOfCollaboration, Company collaboratorToBeAdded, int loggedInUserID)
        {
            this._repository.AddCollaboratorToRepo(eventOfCollaboration, collaboratorToBeAdded, loggedInUserID);
        }

        /// <summary>
        /// Retrieves all collaborators associated with the specified company.
        /// </summary>
        /// <param name="loggedInCompanyId">The unique identifier of the logged in company.</param>
        /// <returns>A list of companies that are collaborators of the specified company.</returns>
        public List<Company> GetAllCollaborators(int loggedInCompanyId)
        {
            return this._repository.GetAllCollaborators(loggedInCompanyId);
        }

        /// <summary>
        /// Retrieves all collaborators associated with the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <returns>A list of companies that are collaborators of the specified event.</returns>
        public List<Company> GetEventCollaborators(int eventId)
        {
            return this._repository.GetEventCollaborators(eventId);
        }
    }
}
