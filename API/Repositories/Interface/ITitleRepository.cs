using API.Models;
using System.Data;

namespace API.Repositories.Interface
{
    public interface ITitleRepository
    {
        IEnumerable<Title> GetAllTitles();
        Title GetTitleById(string id);
        int AddTitles(Title title);
        int UpdateTitle(Title title);
        int DeleteTitleById(string id);
    }
}
