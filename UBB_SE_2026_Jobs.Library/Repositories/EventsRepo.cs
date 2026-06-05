namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class EventsRepo : IEventsRepo
    {
        private readonly JobsDbContext databaseContext;

        public EventsRepo(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc/>
        public void AddEventToRepo(Event eventToBeAdded)
        {
            using var transaction = this.databaseContext.Database.BeginTransaction();

            try
            {
                eventToBeAdded.PostedAt = DateTime.Now;

                var companyIds = eventToBeAdded.Collaborators?.Select(collaborator => collaborator.CompanyId).ToList() ?? new List<int>();
                var newCompanyIds = new List<int>();

                foreach (var companyId in companyIds)
                {
                    bool alreadyCollaborates = this.databaseContext.Collaborators.Any(collaborator => collaborator.CompanyId == companyId);
                    if (!alreadyCollaborates)
                    {
                        newCompanyIds.Add(companyId);
                    }
                }

                // EF Core will automatically insert the Event and its Collaborators
                this.databaseContext.Events.Add(eventToBeAdded);
                this.databaseContext.SaveChanges();

                foreach (var companyId in newCompanyIds)
                {
                    var company = this.databaseContext.Companies.Find(companyId);
                    if (company != null)
                    {
                        company.CollaboratorsCount += 1;
                    }
                }

                if (newCompanyIds.Any())
                {
                    this.databaseContext.SaveChanges();
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
        public void RemoveEventFromRepo(Event eventToBeRemoved)
        {
            var existing = this.databaseContext.Events.Find(eventToBeRemoved.Id);
            if (existing != null)
            {
                this.databaseContext.Events.Remove(existing);
                this.databaseContext.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Event> GetCurrentEventsFromRepo(int? loggedInUser = null)
        {
            var query = this.databaseContext.Events
                .Where(eventEntity => eventEntity.EndDate >= DateTime.Now.Date);

            if (loggedInUser.HasValue)
            {
                query = query.Where(eventEntity => eventEntity.HostCompanyId == loggedInUser);
            }

            var events = query.ToList();
            return new ObservableCollection<Event>(events);
        }

        /// <inheritdoc/>
        public ObservableCollection<Event> GetPastEventsFromRepo(int? loggedInUser = null)
        {
            var query = this.databaseContext.Events
                .Where(eventEntity => eventEntity.EndDate < DateTime.Now.Date);

            if (loggedInUser.HasValue)
            {
                query = query.Where(eventEntity => eventEntity.HostCompanyId == loggedInUser);
            }

            var events = query.ToList();
            return new ObservableCollection<Event>(events);
        }

        /// <inheritdoc/>
        public void UpdateEventToRepo(int eventId, string photo, string title, string description, DateTime start, DateTime end, string location, List<int> collaboratorCompanyIds)
        {
            var existing = this.databaseContext.Events.Find(eventId);
            if (existing == null)
            {
                return;
            }

            existing.Photo = photo;
            existing.Title = title;
            existing.Description = description;
            existing.StartDate = start;
            existing.EndDate = end;
            existing.Location = location;
            existing.PostedAt = DateTime.Now;

            // Sync collaborators
            var existingCollaborators = this.databaseContext.Collaborators.Where(collaborator => collaborator.EventId == eventId).ToList();
            var existingCompanyIds = existingCollaborators.Select(collaborator => collaborator.CompanyId).ToList();

            var toRemove = existingCollaborators.Where(collaborator => !collaboratorCompanyIds.Contains(collaborator.CompanyId)).ToList();
            var toAddIds = collaboratorCompanyIds.Where(collaboratorCompanyId => !existingCompanyIds.Contains(collaboratorCompanyId)).ToList();

            if (toRemove.Any())
            {
                this.databaseContext.Collaborators.RemoveRange(toRemove);
            }

            foreach (var collaboratorCompanyId in toAddIds)
            {
                bool alreadyCollaborates = this.databaseContext.Collaborators
                    .Any(collaborator => collaborator.CompanyId == collaboratorCompanyId);

                var newCollaborator = new Collaborator
                {
                    EventId = eventId,
                    CompanyId = collaboratorCompanyId
                };

                this.databaseContext.Collaborators.Add(newCollaborator);
                this.databaseContext.SaveChanges();

                if (!alreadyCollaborates)
                {
                    var company = this.databaseContext.Companies.Find(collaboratorCompanyId);
                    if (company != null)
                    {
                        company.CollaboratorsCount += 1;
                    }
                }
            }

            this.databaseContext.SaveChanges();
        }
    }
}