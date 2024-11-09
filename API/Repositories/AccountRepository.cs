using API.Context;
using API.Models;
using API.Repositories.Interface;
using API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MyContext _context;
        public AccountRepository(MyContext context)
        {
            _context = context;
        }

        public int AddAccount(AccountVM accountVM)
        {
            // cek jika email sudah terdaftar atau belum
            if (_context.Accounts.Any(e => e.Email == accountVM.Email))
            {
                throw new Exception("Email is already REGISTERED!");
            }

            // validasi sederhana buat ngecek kalo data diisi atau tidak
            if (string.IsNullOrWhiteSpace(accountVM.Email))
            {
                throw new Exception("Email is required!");
            }
            if (string.IsNullOrWhiteSpace(accountVM.Role_Name))
            {
                throw new Exception("Role is required!");
            }
            if (string.IsNullOrWhiteSpace(accountVM.Name))
            {
                throw new Exception("Name is required!");
            }
            if (string.IsNullOrWhiteSpace(accountVM.Phone))
            {
                throw new Exception("Phone is required!");
            }
            if (accountVM.Birth_Date == null)
            {
                throw new Exception("Birth Date is required!");
            }
            if (accountVM.Join_Date == null)
            {
                throw new Exception("Join Date is required!");
            }
            if (accountVM.Current_Limit == null)
            {
                throw new Exception("Current Reimbursement Limit is required!");
            }

            // mapping data untuk table Account
            Account account = new Account();
            account.Id_Account = GenerateCustomIdForAccount();
            account.Email = accountVM.Email;
            account.Role_Name = accountVM.Role_Name;
            var password = "12345";
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            account.Password = BCrypt.Net.BCrypt.HashPassword(password, salt);
            _context.Accounts.Add(account);

            // mapping data untuk table AccountDetail
            AccountDetail accountDetail = new AccountDetail();
            accountDetail.Id_AccountDetail = account.Id_Account;
            accountDetail.Id_Title = accountVM.Id_Title;
            accountDetail.Name = accountVM.Name;
            accountDetail.Phone = accountVM.Phone;
            accountDetail.Gender = accountVM.Gender;
            accountDetail.Birth_Date = accountVM.Birth_Date;
            accountDetail.Join_Date = accountVM.Join_Date;
            accountDetail.Current_Limit = accountVM.Current_Limit;
            _context.AccountDetails.Add(accountDetail);

            return _context.SaveChanges();
        }

        public int DeleteAccount(string email)
        {
            var account = _context.Accounts
                .Where(x => x.Email == email)
                .FirstOrDefault();

            if (account == null)
            {
                throw new Exception("Data not found!");
            }

            var accountDetail = _context.AccountDetails
                .Where(id => id.Id_AccountDetail == account.Id_Account)
                .FirstOrDefault();

            _context.AccountDetails.Remove(accountDetail);
            _context.Accounts.Remove(account);
            return _context.SaveChanges();
        }

        public string GenerateCustomIdForAccount()
        {
            DateTime now = DateTime.Now;
            var customId = "";
            // generate custom id untuk table Account
            var lastRecord = _context.Accounts.Max(x => x.Id_Account);
            if (lastRecord == null)
            {
                customId = $"{now.Year}{now.Month.ToString("D2")}0001";
            }
            else
            {
                var lastRecordId = int.Parse(lastRecord.Substring(7));
                lastRecordId++;
                var number = lastRecordId.ToString("D4");
                customId = $"{now.Year}{now.Month.ToString("D2")}{number}";
            }

            return customId;
        }

        public ShowAccountVM GetAccountByEmail(string email)
        {
            var accountByEmail = _context.Accounts
                .Where(e => e.Email == email)
                .FirstOrDefault();
            if (accountByEmail == null)
            {
                throw new Exception("Data not found!");
            }
            var account = _context.Accounts
                .Include(ad => ad.AccountDetails.Title)
                .Where(id => id.Id_Account == accountByEmail.Id_Account)
                .Select(acc => new ShowAccountVM
                {
                    Id_Account = acc.Id_Account,
                    Email = acc.Email,
                    Role_Name = acc.Role_Name,
                    Title_Name = acc.AccountDetails.Title.Title_Name,
                    Name = acc.AccountDetails.Name,
                    Phone = acc.AccountDetails.Phone,
                    Gender = acc.AccountDetails.Gender,
                    Birth_Date = acc.AccountDetails.Birth_Date.ToString("dd-MM-yyyy") ?? string.Empty,
                    Join_Date = acc.AccountDetails.Join_Date.ToString("dd-MM-yyyy") ?? string.Empty,
                    Current_Limit = acc.AccountDetails.Current_Limit,
                })
                .FirstOrDefault();
            return account;
        }

        public IEnumerable<ShowAccountVM> GetAllAccounts()
        {
             var account = _context.Accounts
                .Include(ad => ad.AccountDetails.Title)
                .Select(acc => new ShowAccountVM
                {
                    Id_Account = acc.Id_Account,
                    Email = acc.Email,
                    Role_Name = acc.Role_Name,
                    Title_Name = acc.AccountDetails.Title.Title_Name,
                    Name = acc.AccountDetails.Name,
                    Phone = acc.AccountDetails.Phone,
                    Gender = acc.AccountDetails.Gender,
                    Birth_Date = acc.AccountDetails.Birth_Date.ToString("dd-MM-yyyy") ?? string.Empty,
                    Join_Date = acc.AccountDetails.Join_Date.ToString("dd-MM-yyyy") ?? string.Empty,
                    Current_Limit = acc.AccountDetails.Current_Limit,
                })
                .ToList();
            if (account == null)
            {
                throw new Exception("Data not found!");
            }
            return account;
        }

        public AccountVM GetLastInsertedAccount()
        {
            var lastInsertedAccount = _context.Accounts
                .Include(a => a.AccountDetails)
                .OrderByDescending(id => id.Id_Account)
                .FirstOrDefault();
            if(lastInsertedAccount == null)
            {
                return null;
            }

            return new AccountVM
            {
                Email = lastInsertedAccount.Email,
                Role_Name = lastInsertedAccount.Role_Name,
                Id_Title = lastInsertedAccount.AccountDetails.Id_Title,
                Name = lastInsertedAccount.AccountDetails.Name,
                Phone = lastInsertedAccount.AccountDetails.Phone,
                Gender = lastInsertedAccount.AccountDetails.Gender,
                Birth_Date = lastInsertedAccount.AccountDetails.Birth_Date,
                Join_Date = lastInsertedAccount.AccountDetails.Join_Date,
                Current_Limit = lastInsertedAccount.AccountDetails.Current_Limit,
            };
        }

        public int UpdateAccount(AccountVM accountVM)
        {
            var account = _context.Accounts
                .Where(e => e.Email == accountVM.Email)
                .FirstOrDefault();
            if (account == null)
            {
                throw new Exception("Data not found!");
            }

            var accountDetail = _context.AccountDetails
                .Where(id => id.Id_AccountDetail == account.Id_Account)
                .FirstOrDefault();

            account.Email = accountVM.Email;
            account.Role_Name = accountVM.Role_Name;
            accountDetail.Id_Title = accountVM.Id_Title;
            accountDetail.Name = accountVM.Name;
            accountDetail.Phone = accountVM.Phone;
            accountDetail.Gender = accountVM.Gender;
            accountDetail.Birth_Date = accountVM.Birth_Date;
            accountDetail.Join_Date = accountVM.Join_Date;
            accountDetail.Current_Limit = accountVM.Current_Limit;

            _context.Entry(account).State = EntityState.Modified;
            _context.Entry(account).State = EntityState.Detached;
            _context.Entry(accountDetail).State = EntityState.Modified;

            return _context.SaveChanges();
        }
    }
}
