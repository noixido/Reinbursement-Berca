using API.ViewModels;
using System.Data;

namespace API.Repositories.Interface
{
    public interface IReimbursementRepository
    {
        IEnumerable<ReimbursementVM> GetAllReimbursementByAccount(string id);
        ReimbursementVM GetReimbursementById(string id);
        Task<int> AddReimbursement(ReimbursementVM reimbursement);
        IEnumerable<ReimbursementVM> GetAllReimbursements();
        int UpdateReimbursementStatusByHR(string id, ChangeStatusReimbursement changeReimbursement, bool isApprove);
        int UpdateReimbursementStatusByFinance(string id, ChangeStatusReimbursement changeReimbursement, bool isApprove);
        IEnumerable<ReimbursementVM> GetAllReimbursementsByStatus(string status);

    }
}
