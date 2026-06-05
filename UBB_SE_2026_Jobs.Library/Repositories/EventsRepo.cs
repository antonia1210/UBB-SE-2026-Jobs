namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class EventsRepo : IEventsRepo
    {
        private readonly JobsDbContext JobsDbContext;

        public EventsRepo(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc/>
        public void AddEventToRepo(Event eventToBeAdded)
        {
            using var transaction = this.JobsDbContext.Database.BeginTransaction();

            try
            {
                eventToBeAdded.PostedAt = DateTime.Now;

                var companyIds = eventToBeAdded.Collaborators?.Select(c => c.CompanyId).ToList() ?? new List<int>();
                var newCompanyIds = new List<int>();
                
                foreach (var cid in companyIds)
                {
                    bool alreadyCollaborates = this.JobsDbContext.Collaborators.Any(c => c.CompanyId == cid);
                    if (!alreadyCollaborates)
                    {
                        newCompanyIds.Add(cid);
                    }
                }

                // EF Core will automatically insert the Event and its Collaborators
                this.JobsDbContext.Events.Add(eventToBeAdded);
                this.JobsDbContext.SaveChanges();

                foreach (var cid in newCompanyIds)
                {
                    var company = this.JobsDbContext.Companies.Find(cid);
                    if (company != null)
                    {
                        company.CollaboratorsCount += 1;
                    }
                }
                
                if (newCompanyIds.Any())
                {
                    this.JobsDbContext.SaveChanges();
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
            var existing = this.JobsDbContext.Events.Find(eventToBeRemoved.Id);
            if (existing != null)
            {
                this.JobsDbContext.Events.Remove(existing);
                this.JobsDbContext.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public ObservableCollection<Event> GetCurrentEventsFromRepo(int? loggedInUser=null)
        {
            var query = this.JobsDbContext.Events
                .Where(e => e.EndDate >= DateTime.Now.Date);

                if (loggedInUser.HasValue)
                {
                    query = query.Where(e => e.HostCompanyId == loggedInUser);
                }

                var events = query.ToList();
                return new ObservableCollection<Event>(events);
        }

        /// <inheritdoc/>
        public ObservableCollection<Event> GetPastEventsFromRepo(int? loggedInUser=null)
        {
            var query = this.JobsDbContext.Events
                .Where(e => e.EndDate < DateTime.Now.Date);

                if (loggedInUser.HasValue)
                {
                    query = query.Where(e => e.HostCompanyId == loggedInUser);
                }

                var events = query.ToList();
                return new ObservableCollection<Event>(events);
        }

        /// <inheritdoc/>
        public void UpdateEventToRepo(int id, string photo, string title, string description, DateTime start, DateTime end, string location, List<int> collaboratorCompanyIds)
        {
            var existing = this.JobsDbContext.Events.Find(id);
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
            var existingCollaborators = this.JobsDbContext.Collaborators.Where(c => c.EventId == id).ToList();
            var existingCompanyIds = existingCollaborators.Select(c => c.CompanyId).ToList();

            var toRemove = existingCollaborators.Where(c => !collaboratorCompanyIds.Contains(c.CompanyId)).ToList();
            var toAddIds = collaboratorCompanyIds.Where(cId => !existingCompanyIds.Contains(cId)).ToList();

            if (toRemove.Any())
            {
                this.JobsDbContext.Collaborators.RemoveRange(toRemove);
            }

            foreach (var companyId in toAddIds)
            {
                bool alreadyCollaborates = this.JobsDbContext.Collaborators
                    .Any(c => c.CompanyId == companyId);

                var newCollaborator = new Collaborator
                {
                    EventId = id,
                    CompanyId = companyId
                };

                this.JobsDbContext.Collaborators.Add(newCollaborator);
                this.JobsDbContext.SaveChanges(); // save to ensure it gets an ID if needed before count update

                if (!alreadyCollaborates)
                {
                    var company = this.JobsDbContext.Companies.Find(companyId);
                    if (company != null)
                    {
                        company.CollaboratorsCount += 1;
                    }
                }
            }

            this.JobsDbContext.SaveChanges();
        }
    }
}

