using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Library.Services
{
    public class TestsAuthService : ITestsAuthService
    {
        private const string Issuer = "UBB-SE-2026";
        private const string Audience = "UBB-SE-Client";

        private readonly ITestsCompanyRepository companyRepository;
        private readonly JobsDbContext dbContext;
        private readonly string secretKey;

        public TestsAuthService(ITestsCompanyRepository companyRepository, JobsDbContext dbContext, IConfiguration configuration)
        {
            this.companyRepository = companyRepository;
            this.dbContext = dbContext;
            this.secretKey = configuration["TiJwt:Key"]
                ?? throw new InvalidOperationException("Missing 'TiJwt:Key' configuration. Add it to appsettings.json.");
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await this.dbContext.Users
                .FirstOrDefaultAsync(user => user.Email == dto.Email);

            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            var role = await this.ResolveRoleAsync(user.Id);
            user.Role = role;

            var companyId = await this.GetCompanyIdForUserAsync(user.Id);

            return new AuthResponseDto
            {
                Token = this.GenerateJwt(user),
                Role = role,
                Name = user.Name,
                UserId = user.Id,
                CompanyId = companyId,
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // In the merged setup, PussyCats API owns the Users table and always creates
            // the user before calling this endpoint. We never INSERT into Users here.
            var user = await this.dbContext.Users
                .FirstOrDefaultAsync(user => user.Email == dto.Email);

            if (user == null) return null;

            int? companyId = null;
            if (dto.Role == "Recruiter")
            {
                if (!dto.CompanyId.HasValue) return null;

                var company = this.companyRepository.GetById(dto.CompanyId.Value);
                if (company == null) return null;

                bool alreadyRecruiter = await this.dbContext.Recruiters
                    .AnyAsync(recruiter => recruiter.UserId == user.Id);
                if (!alreadyRecruiter)
                {
                    this.dbContext.Recruiters.Add(new Recruiter
                    {
                        CompanyId = company.CompanyId,
                        UserId = user.Id,
                        CompanyName = company.Name,
                        Company = company,
                    });
                    await this.dbContext.SaveChangesAsync();
                }
                companyId = company.CompanyId;
            }

            user.Role = dto.Role;
            return new AuthResponseDto
            {
                Token = this.GenerateJwt(user),
                Role = dto.Role,
                Name = user.Name,
                UserId = user.Id,
                CompanyId = companyId,
            };
        }

        private async Task<string> ResolveRoleAsync(int userId)
        {
            bool isRecruiter = await this.dbContext.Recruiters
                .AnyAsync(recruiter => recruiter.UserId == userId);
            return isRecruiter ? "Recruiter" : "Candidate";
        }

        private async Task<int?> GetCompanyIdForUserAsync(int userId)
        {
            var recruiter = await this.dbContext.Recruiters
                .FirstOrDefaultAsync(recruiter => recruiter.UserId == userId);
            return recruiter?.CompanyId;
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var token = new JwtSecurityToken(
                issuer: Issuer, audience: Audience, claims: claims,
                expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

