using API.Context;
using API.Models;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Repositories
{
    public class TitleRepositories : ITitleRepository
    {
        public const int NOTFOUND = -3;
        public const int INUSE = -2;

        private readonly MyContext _myContext;

        public TitleRepositories(MyContext myContext) 
        { 
            _myContext = myContext;
        }
        public int AddTitles(Title title)
        {
            var totalTitle = _myContext.Titles.Count();
            var idTitle = $"T{++totalTitle:D3}";
            title.Id_Title = idTitle;

            _myContext.Titles.Add(title);
            return _myContext.SaveChanges();
        }

        public int DeleteTitleById(string id)
        {
            var titleToDelete = GetTitleById(id);
            if (titleToDelete == null)
            {
                return NOTFOUND;
            }

            bool isTitleInUse = _myContext.AccountDetails.Any(ad => ad.Id_Title == id);
            if (isTitleInUse)
            {
                return INUSE;
            }
            _myContext.Titles.Remove(titleToDelete);
            return _myContext.SaveChanges();
        }

        public IEnumerable<Title> GetAllTitles()
        {
            return _myContext.Titles.ToList();
        }

        public Title GetTitleById(string id)
        {
            return _myContext.Titles.FirstOrDefault(u => u.Id_Title == id);
        }

        public int UpdateTitle(Title title)
        {
            var check = GetTitleById(title.Id_Title);
            if (check == null)
            {
                return NOTFOUND;
            }

            _myContext.Entry(check).State = EntityState.Detached;
            _myContext.Entry(title).State = EntityState.Modified;

            return _myContext.SaveChanges();
        }
    }
}
