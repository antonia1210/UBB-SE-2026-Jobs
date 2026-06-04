namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

    public interface ICompanyRepo
    {
        public void PrintAll();
        ObservableCollection<Company> GetAll();
        Company? GetById(int companyId);
        void Add(Company c);
        void Remove(int companyID);
        Company? GetCompanyByName(string companyName);
        void Update(Company c);
        Game? GetGame();
        void SaveGame(Game game);
    }
}
