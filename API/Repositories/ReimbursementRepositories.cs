using API.Context;
using API.Models;
using API.Repositories.Interface;
using API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ReimbursementRepositories : IReimbursementRepository
    {
        public const int NOTFOUND = -3;

        private readonly MyContext _myContext;

        public ReimbursementRepositories(MyContext myContext)
        {
            _myContext = myContext;
        }

        public int AddReimbursement(ReimbursementVM reimbursement)
        {
            var totalReimbursement = _myContext.Reimbursements.Count();
            var idReimbursement = $"R{++totalReimbursement:D7}";
            reimbursement.Id_Reimbursement = idReimbursement;

            Reimbursement newReimbursement = new Reimbursement
            {
                Id_Reimbursement = idReimbursement,
                Id_Category = reimbursement.Id_Category,
                Evidence = reimbursement.Evidence,
                Amount = (float)reimbursement.Amount,
                Approve_Amount = 0,
                Note = reimbursement.Note,
                Status = "progress in hr",
                Submit_Date = (DateTime)reimbursement.Submit_Date,
            };
            _myContext.Reimbursements.Add(newReimbursement);

            ReimbursementProfiling newReimbursementProfiling = new ReimbursementProfiling
            {
                Id_Account = reimbursement.Id_Account,
                Id_Reimbursement = idReimbursement
            };
            _myContext.ReimbursementProfilings.Add(newReimbursementProfiling);

            return _myContext.SaveChanges();
        }

        public IEnumerable<ReimbursementVM> GetAllReimbursementByAccount(string id)
        {
            return _myContext.Reimbursements
            .Include(re => re.ReimbursementProfilings)
                .ThenInclude(rp => rp.Account).ThenInclude(a => a.AccountDetails)
            .Where(re => re.ReimbursementProfilings.Any(rp => rp.Id_Account == id))
            .Select(re => new ReimbursementVM
            {
                Id_Account = re.ReimbursementProfilings.FirstOrDefault(rp => rp.Id_Account == id).Id_Account,
                Id_Reimbursement = re.Id_Reimbursement,
                Name = re.ReimbursementProfilings.FirstOrDefault(Account => Account.Id_Account == id).Account.AccountDetails.Name,
                Id_Category = re.Id_Category,
                Category_Name = re.Category.Category_Name,
                Evidence = re.Evidence,
                Amount = re.Amount,
                Approve_Amount = re.Approve_Amount,
                Note = re.Note,
                Status = re.Status,
                Submit_Date = re.Submit_Date
            })
            .ToList();
        }

        public ReimbursementVM GetReimbursementById(string id)
        {
            
            var reimbursement = _myContext.Reimbursements
                .Include(re => re.ReimbursementProfilings)
                    .ThenInclude(rp => rp.Account).ThenInclude(a => a.AccountDetails)
                .Include(re => re.Category)
                .FirstOrDefault(re => re.Id_Reimbursement == id);

            if (reimbursement == null)
            {
                return null;
            }

            ReimbursementVM reimbursementVM = new ReimbursementVM()
            {
                Id_Account = reimbursement.ReimbursementProfilings?.FirstOrDefault().Id_Account,
                Id_Reimbursement = reimbursement.Id_Reimbursement,
                Name = reimbursement.ReimbursementProfilings?.FirstOrDefault().Account.AccountDetails.Name,
                Id_Category = reimbursement.Id_Category,
                Category_Name = reimbursement.Category?.Category_Name,
                Evidence = reimbursement.Evidence,
                Amount = reimbursement.Amount,
                Approve_Amount = reimbursement.Approve_Amount,
                Note = reimbursement.Note,
                Status = reimbursement.Status,
                Submit_Date = reimbursement.Submit_Date,
            };

            return reimbursementVM;
        }

        public IEnumerable<ReimbursementVM> GetAllReimbursements()
        {
            return _myContext.Reimbursements
            .Include(re => re.ReimbursementProfilings)
                .ThenInclude(rp => rp.Account).ThenInclude(a => a.AccountDetails)
            .Include(re => re.Category)
            .Select(re => new ReimbursementVM
            {
                Id_Account = re.ReimbursementProfilings.FirstOrDefault().Id_Account,
                Id_Reimbursement = re.Id_Reimbursement,
                Name = re.ReimbursementProfilings.FirstOrDefault().Account.AccountDetails.Name,
                Id_Category = re.Id_Category,
                Category_Name = re.Category.Category_Name,
                Evidence = re.Evidence,
                Amount = re.Amount,
                Approve_Amount = re.Approve_Amount,
                Note = re.Note,
                Status = re.Status,
                Submit_Date = re.Submit_Date
            })
            .ToList();
        }

        public int UpdateReimbursementStatusByHR(string id, ChangeStatusReimbursement changeReimbursement, bool isApprove)
        {
            var reimbursement = _myContext.Reimbursements.FirstOrDefault(re => re.Id_Reimbursement == id);
            if (reimbursement == null)
            {
                return NOTFOUND;
            }

            if (isApprove)
            {
                reimbursement.Status = "progress in finance";
            }
            else
            {
                reimbursement.Status = "declined by HR";
            }

            reimbursement.Note = changeReimbursement.Note;

            return _myContext.SaveChanges();
        }

        public int UpdateReimbursementStatusByFinance(string id, ChangeStatusReimbursement approveReimbursement, bool isAprrove)
        {
            var reimbursement = _myContext.Reimbursements.FirstOrDefault(re => re.Id_Reimbursement == id);
            if (reimbursement == null)
            {
                return NOTFOUND;
            }

            if (isAprrove)
            {
                reimbursement.Status = "approved";
                reimbursement.Approve_Amount = approveReimbursement.Approve_Amount;
            }
            else
            {
                reimbursement.Status = "declined by finance";
            }
            reimbursement.Note = approveReimbursement.Note;

            return _myContext.SaveChanges();
        }

        public IEnumerable<ReimbursementVM> GetAllReimbursementsByStatus(string status)
        {
            var reimbursements = _myContext.Reimbursements
                .Include(re => re.ReimbursementProfilings)
                    .ThenInclude(rp => rp.Account).ThenInclude(a => a.AccountDetails)
                .Include(re => re.Category)
                .Where(re => re.Status == status)
                .Select(re => new ReimbursementVM
                {
                    Id_Account = re.ReimbursementProfilings.FirstOrDefault().Id_Account,
                    Id_Reimbursement = re.Id_Reimbursement,
                    Name = re.ReimbursementProfilings.FirstOrDefault().Account.AccountDetails.Name,
                    Id_Category = re.Id_Category,
                    Category_Name = re.Category.Category_Name,
                    Evidence = re.Evidence,
                    Amount = re.Amount,
                    Approve_Amount = re.Approve_Amount,
                    Note = re.Note,
                    Status = re.Status,
                    Submit_Date = re.Submit_Date,
                })
                .ToList();

            return reimbursements;
        }


    }
}
