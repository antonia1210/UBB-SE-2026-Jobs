namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class CollaboratorsRepo : ICollaboratorsRepo
    {
        private readonly JobsDbContext JobsDbContext;

        public CollaboratorsRepo(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc/>
        public void AddCollaboratorToRepo(Event eventOfCollaboration, Company collaboratorToBeAdded, int loggedInUserID)
        {
            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                this.JobsDbContext.Collaborators.Add(new Collaborator
                {
                    EventId = eventOfCollaboration.Id,
                    CompanyId = collaboratorToBeAdded.CompanyId,
                });

                this.JobsDbContext.SaveChanges();

                int existingCount = this.JobsDbContext.Collaborators
                    .Include(c => c.Event)
                    .Count(c => c.Event.HostCompanyId == loggedInUserID
                        && c.CompanyId == collaboratorToBeAdded.CompanyId);

                bool isNewCollaborator = existingCount == 1;

                if (isNewCollaborator)
                {
                    var hostCompany = this.JobsDbContext.Companies.Find(loggedInUserID);
                    if (hostCompany != null)
                    {
                        hostCompany.CollaboratorsCount += 1;
                        this.JobsDbContext.SaveChanges();
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
            var companyIds = this.JobsDbContext.Collaborators
                .Include(c => c.Event)
                .Where(c => c.Event.HostCompanyId == loggedInCompanyId)
                .Select(c => c.CompanyId)
                .Distinct()
                .ToList();

            return this.JobsDbContext.Companies
                .Where(c => companyIds.Contains(c.CompanyId))
                .ToList();
        }
    }
}

