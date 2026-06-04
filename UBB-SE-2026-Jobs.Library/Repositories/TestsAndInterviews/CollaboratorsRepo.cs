namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Data;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces;

    public class CollaboratorsRepo : ICollaboratorsRepo
    {
        private readonly AppDbContext appDbContext;

        public CollaboratorsRepo(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc/>
        public void AddCollaboratorToRepo(Event eventOfCollaboration, Company collaboratorToBeAdded, int loggedInUserID)
        {
            using var transaction = this.appDbContext.Database.BeginTransaction();

            try
            {
                this.appDbContext.Collaborators.Add(new Collaborator
                {
                    EventId = eventOfCollaboration.Id,
                    CompanyId = collaboratorToBeAdded.CompanyId,
                });

                this.appDbContext.SaveChanges();

                int existingCount = this.appDbContext.Collaborators
                    .Include(c => c.Event)
                    .Count(c => c.Event.HostCompanyId == loggedInUserID
                        && c.CompanyId == collaboratorToBeAdded.CompanyId);

                bool isNewCollaborator = existingCount == 1;

                if (isNewCollaborator)
                {
                    var hostCompany = this.appDbContext.Companies.Find(loggedInUserID);
                    if (hostCompany != null)
                    {
                        hostCompany.CollaboratorsCount += 1;
                        this.appDbContext.SaveChanges();
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <inheritdoc/>
        public List<Company> GetAllCollaborators(int loggedInCompanyId)
        {
            var companyIds = this.appDbContext.Collaborators
                .Include(c => c.Event)
                .Where(c => c.Event.HostCompanyId == loggedInCompanyId)
                .Select(c => c.CompanyId)
                .Distinct()
                .ToList();

            return this.appDbContext.Companies
                .Where(c => companyIds.Contains(c.CompanyId))
                .ToList();
        }
    }
}