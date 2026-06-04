using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;
namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services
{
    public class SessionService
    {
        public Company LoggedInUser { get; }

        public SessionService(Company user)
        {
            this.LoggedInUser = user;
        }
    }
}
