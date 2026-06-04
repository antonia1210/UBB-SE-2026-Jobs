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

                this.JobsDbContext.Events.Add(eventToBeAdded);
                this.JobsDbContext.SaveChanges();

                if (eventToBeAdded.Collaborators != null)
                {
                    foreach (var collaborator in eventToBeAdded.Collaborators)
                    {
                        bool alreadyCollaborates = this.JobsDbContext.Collaborators
                            .Any(c => c.CompanyId == collaborator.CompanyId);

                        collaborator.EventId = eventToBeAdded.Id;
                        this.JobsDbContext.Collaborators.Add(collaborator);
                        this.JobsDbContext.SaveChanges();

                        if (!alreadyCollaborates)
                        {
                            var company = this.JobsDbContext.Companies.Find(collaborator.CompanyId);
                            if (company != null)
                            {
                                company.CollaboratorsCount += 1;
                                this.JobsDbContext.SaveChanges();
                            }
                        }
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
        public void UpdateEventToRepo(int id, string photo, string title, string description, DateTime start, DateTime end, string location)
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

            this.JobsDbContext.SaveChanges();
        }
    }
}

