using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBB_SE_2026_Jobs.Library.Repositories.Portal;
using UBB_SE_2026_Jobs.Library.Services.Portal;
using UBB_SE_2026_Jobs.Library.Domain.Portal;
namespace UBB_SE_2026_Jobs.Library.Services.Portal
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
