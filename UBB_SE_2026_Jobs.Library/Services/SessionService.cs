using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.Domain;
namespace UBB_SE_2026_Jobs.Library.Services
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
