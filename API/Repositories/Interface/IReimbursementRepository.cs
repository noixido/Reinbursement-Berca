using API.ViewModels;
using System.Data;

namespace API.Repositories.Interface
{
    public interface IReimbursementRepository
    {
        IEnumerable<ReimbursementVM> GetAllReimbursementByAccount(string id);
        ReimbursementVM GetReimbursementById(string id);
        int AddReimbursement(ReimbursementVM reimbursement);
    }
}
