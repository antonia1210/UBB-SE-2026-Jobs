namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    public class TestsCompanyRepository : ITestsCompanyRepository
    {
        private readonly JobsDbContext databaseContext;
        private Company? currentCompany;

        public TestsCompanyRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        private void ValidateRequiredFields(Company company)
        {
            if (company is null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            if (string.IsNullOrWhiteSpace(company.Name))
            {
                throw new ArgumentException("Company name is required.", nameof(company));
            }
            if (string.IsNullOrWhiteSpace(company.CompanyLogoPath))
            {
                throw new ArgumentException("Company logo url/path is required.", nameof(company));
            }
        }

        private static Game MapGame(Company company)
        {
            var buddy = new Buddy(
                company.AvatarId ?? 0,
                company.BuddyName ?? string.Empty,
                company.BuddyDescription ?? string.Empty);

            var scenarios = new List<Scenario>();

            if (!string.IsNullOrEmpty(company.Scen1Text))
            {
                var scenario1 = new Scenario(company.Scen1Text);
                scenario1.Choices.Add(new AdviceChoice(company.Scen1Answer1 ?? string.Empty, company.Scen1Reaction1 ?? string.Empty));
                scenario1.Choices.Add(new AdviceChoice(company.Scen1Answer2 ?? string.Empty, company.Scen1Reaction2 ?? string.Empty));
                scenario1.Choices.Add(new AdviceChoice(company.Scen1Answer3 ?? string.Empty, company.Scen1Reaction3 ?? string.Empty));
                scenarios.Add(scenario1);
            }

            if (!string.IsNullOrEmpty(company.Scen2Text))
            {
                var scenario2 = new Scenario(company.Scen2Text);
                scenario2.Choices.Add(new AdviceChoice(company.Scen2Answer1 ?? string.Empty, company.Scen2Reaction1 ?? string.Empty));
                scenario2.Choices.Add(new AdviceChoice(company.Scen2Answer2 ?? string.Empty, company.Scen2Reaction2 ?? string.Empty));
                scenario2.Choices.Add(new AdviceChoice(company.Scen2Answer3 ?? string.Empty, company.Scen2Reaction3 ?? string.Empty));
                scenarios.Add(scenario2);
            }

            return new Game(buddy, scenarios, company.FinalQuote ?? string.Empty, true);
        }

        public Game? GetGame()
        {
            if (this.currentCompany == null)
            {
                return null;
            }

            return this.currentCompany.Game;
        }

        public void SaveGame(Game game)
        {
            if (this.currentCompany == null)
            {
                throw new InvalidOperationException("Nu exista o companie curenta selectata.");
            }

            this.currentCompany.Game = game;

            var existingCompany = this.databaseContext.Companies.Find(this.currentCompany.CompanyId);
            if (existingCompany == null)
            {
                return;
            }

            existingCompany.BuddyName = game.Buddy.Name;
            existingCompany.BuddyDescription = game.Buddy.Introduction;
            existingCompany.AvatarId = game.Buddy.Id;
            existingCompany.FinalQuote = game.Conclusion;
            existingCompany.Scen1Text = game.Scenarios[0].Description;
            existingCompany.Scen1Answer1 = game.Scenarios[0].Choices[0].Advice;
            existingCompany.Scen1Answer2 = game.Scenarios[0].Choices[1].Advice;
            existingCompany.Scen1Answer3 = game.Scenarios[0].Choices[2].Advice;
            existingCompany.Scen1Reaction1 = game.Scenarios[0].Choices[0].Feedback;
            existingCompany.Scen1Reaction2 = game.Scenarios[0].Choices[1].Feedback;
            existingCompany.Scen1Reaction3 = game.Scenarios[0].Choices[2].Feedback;
            existingCompany.Scen2Text = game.Scenarios[1].Description;
            existingCompany.Scen2Answer1 = game.Scenarios[1].Choices[0].Advice;
            existingCompany.Scen2Answer2 = game.Scenarios[1].Choices[1].Advice;
            existingCompany.Scen2Answer3 = game.Scenarios[1].Choices[2].Advice;
            existingCompany.Scen2Reaction1 = game.Scenarios[1].Choices[0].Feedback;
            existingCompany.Scen2Reaction2 = game.Scenarios[1].Choices[1].Feedback;
            existingCompany.Scen2Reaction3 = game.Scenarios[1].Choices[2].Feedback;

            this.databaseContext.SaveChanges();
        }

        public void PrintAll()
        {
            var companies = this.databaseContext.Companies.ToList();
            foreach (var company in companies)
            {
                System.Diagnostics.Debug.WriteLine($"{company} ");
            }
        }

        ObservableCollection<Company> ITestsCompanyRepository.GetAll()
        {
            var companies = this.databaseContext.Companies.ToList();
            foreach (var company in companies)
            {
                company.Game = MapGame(company);
            }

            return new ObservableCollection<Company>(companies);
        }

        Company? ITestsCompanyRepository.GetById(int companyId)
        {
            var company = this.databaseContext.Companies
                .FirstOrDefault(company => company.CompanyId == companyId);

            if (company == null)
            {
                return null;
            }

            company.Game = MapGame(company);
            this.currentCompany = company;
            return company;
        }

        void ITestsCompanyRepository.Add(Company company)
        {
            ValidateRequiredFields(company);

            this.databaseContext.Companies.Add(company);
            this.databaseContext.SaveChanges();
        }

        void ITestsCompanyRepository.Remove(int companyId)
        {
            var company = this.databaseContext.Companies.Find(companyId);
            if (company != null)
            {
                this.databaseContext.Companies.Remove(company);
                this.databaseContext.SaveChanges();
            }
        }

        void ITestsCompanyRepository.Update(Company company)
        {
            ValidateRequiredFields(company);

            var existingCompany = this.databaseContext.Companies.Find(company.CompanyId);
            if (existingCompany == null)
            {
                throw new InvalidOperationException($"No company found with id '{company.CompanyId}' to update.");
            }

            existingCompany.Name = company.Name;
            existingCompany.AboutUs = company.AboutUs;
            existingCompany.ProfilePicturePath = company.ProfilePicturePath;
            existingCompany.CompanyLogoPath = company.CompanyLogoPath;
            existingCompany.Location = company.Location;
            existingCompany.Email = company.Email;
            existingCompany.BuddyName = company.BuddyName;
            existingCompany.BuddyDescription = company.BuddyDescription;
            existingCompany.AvatarId = company.AvatarId;
            existingCompany.FinalQuote = company.FinalQuote;
            existingCompany.Scen1Text = company.Scen1Text;
            existingCompany.Scen1Answer1 = company.Scen1Answer1;
            existingCompany.Scen1Answer2 = company.Scen1Answer2;
            existingCompany.Scen1Answer3 = company.Scen1Answer3;
            existingCompany.Scen1Reaction1 = company.Scen1Reaction1;
            existingCompany.Scen1Reaction2 = company.Scen1Reaction2;
            existingCompany.Scen1Reaction3 = company.Scen1Reaction3;
            existingCompany.Scen2Text = company.Scen2Text;
            existingCompany.Scen2Answer1 = company.Scen2Answer1;
            existingCompany.Scen2Answer2 = company.Scen2Answer2;
            existingCompany.Scen2Answer3 = company.Scen2Answer3;
            existingCompany.Scen2Reaction1 = company.Scen2Reaction1;
            existingCompany.Scen2Reaction2 = company.Scen2Reaction2;
            existingCompany.Scen2Reaction3 = company.Scen2Reaction3;

            this.databaseContext.SaveChanges();
        }

        public Company? GetCompanyByName(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return null;
            }

            var company = this.databaseContext.Companies
                .FirstOrDefault(company => company.Name == companyName);

            if (company != null)
            {
                company.Game = MapGame(company);
            }

            return company;
        }

        public List<Company> GetByRecruiter(int recruiterId)
        {
            return this.databaseContext.Recruiters
                .Where(recruiter => recruiter.UserId == recruiterId)
                .Join(
                    this.databaseContext.Companies,
                    recruiter => recruiter.CompanyId,
                    company => company.CompanyId,
                    (recruiter, company) => company)
                .ToList();
        }
    }
}