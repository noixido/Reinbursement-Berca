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
            accountDetail.Limit_Updated_At = DateTime.Now;

            var title = _context.Titles
                .Where(id => id.Id_Title == accountDetail.Id_Title)
                .FirstOrDefault();

            //DateTime now = DateTime.Now;
            //float totalWorkMonth = (now.Year - accountDetail.Join_Date.Year) * 12 + now.Month - accountDetail.Join_Date.Month;
            //if(totalWorkMonth >= 12)
            //{
            //    accountDetail.Current_Limit = title.Reimburse_Limit;
            //}
            //else
            //{
            //    var limit = (float)(totalWorkMonth / 12) * title.Reimburse_Limit;
            //    accountDetail.Current_Limit = (float)Math.Round(limit);
            //}

            DateTime endOfTheYear = new DateTime(DateTime.Now.Year, 12, 31);
            float accountAge = ((endOfTheYear.Year - accountDetail.Join_Date.Year) * 12) + (endOfTheYear.Month - accountDetail.Join_Date.Month);
            float totalMonthInAYear = 12;
            accountAge = Math.Max(accountAge, 0);

            if (accountAge >= totalMonthInAYear)
            {
                accountDetail.Current_Limit = title.Reimburse_Limit;
            }
            else
            {
                float limit = (float)(title.Reimburse_Limit * accountAge / totalMonthInAYear);
                accountDetail.Current_Limit = (float)Math.Round(limit);
            }

            accountDetail.IsEmployee = 1;
            _context.AccountDetails.Add(accountDetail);

            return _context.SaveChanges();
        }

        public int ChangePassword(ChangePasswordVM changePasswordVM)
        {
            var account = _context.Accounts
                .Where(e => e.Email == changePasswordVM.Email)
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .FirstOrDefault();
            if (account == null)
            {
                throw new Exception("Data not found!");
            }

            var checkPassword = BCrypt.Net.BCrypt.Verify(changePasswordVM.OldPassword, account.Password);
            if(!checkPassword)
            {
                throw new Exception("Old password is incorrect!");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            account.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordVM.NewPassword, salt);

            _context.Entry(account).State = EntityState.Modified;
            return _context.SaveChanges();
        }
        public int ResetPassword(string email)
        {
            var account = _context.Accounts
                .Where(e => e.Email == email)
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .FirstOrDefault();
            if (account == null)
            {
                throw new Exception("Data not found!");
            }


            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            account.Password = BCrypt.Net.BCrypt.HashPassword("12345", salt);

            _context.Entry(account).State = EntityState.Modified;
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
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
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
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .OrderByDescending(ia => ia.Id_Account)
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

        public LastInsertedAccountVM GetLastInsertedAccount()
        {
            var lastInsertedAccount = _context.Accounts
                .Include(a => a.AccountDetails)
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .OrderByDescending(id => id.Id_Account)
                .FirstOrDefault();
            if(lastInsertedAccount == null)
            {
                return null;
            }

            return new LastInsertedAccountVM
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
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
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

            _context.Entry(account).State = EntityState.Modified;
            _context.Entry(account).State = EntityState.Detached;
            _context.Entry(accountDetail).State = EntityState.Modified;

            return _context.SaveChanges();
        }

        public int DeleteAccount(string email)
        {
            var account = _context.Accounts
                .Where(x => x.Email == email)
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .FirstOrDefault();

            if (account == null)
            {
                throw new Exception("Data not found!");
            }

            var accountDetail = _context.AccountDetails
                .Where(id => id.Id_AccountDetail == account.Id_Account)
                .FirstOrDefault();

            accountDetail.IsEmployee = 0;

            _context.Entry(accountDetail).State = EntityState.Modified;

            //_context.AccountDetails.Remove(accountDetail);
            //_context.Accounts.Remove(account);
            return _context.SaveChanges();
        }

        public ShowAccountForUpdateVM GetAccountByEmailForUpdate(string email)
        {
            var accountByEmail = _context.Accounts
                .Where(e => e.Email == email)
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .FirstOrDefault();
            if (accountByEmail == null)
            {
                throw new Exception("Data not found!");
            }
            var account = _context.Accounts
                .Include(ad => ad.AccountDetails.Title)
                .Where(id => id.Id_Account == accountByEmail.Id_Account)
                .Select(acc => new ShowAccountForUpdateVM
                {
                    Id_Account = acc.Id_Account,
                    Email = acc.Email,
                    Role_Name = acc.Role_Name,
                    Id_Title = acc.AccountDetails.Id_Title,
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

        public async Task UpdateLimitPeriodically()
        {
            var now = DateTime.Now;
            var accountDetails = _context.AccountDetails.Where(ie => ie.IsEmployee == 1).ToList();

            foreach (var account in accountDetails)
            {
                if(now.Day != account.Limit_Updated_At.Day)
                {
                    var janFirst = new DateTime(now.Year + 1, 1, 1);
                    var title = _context.Titles.FirstOrDefault(t => t.Id_Title == account.Id_Title);
                    if (now == janFirst)
                    {
                        account.Current_Limit = title.Reimburse_Limit;
                    }

                    // Perbarui tanggal terakhir Limit_Updated_At
                    account.Limit_Updated_At = now;
                }
                
                //    // Hitung umur akun dalam bulan sejak Join_Date
                //    float accountAgeInMonths = ((now.Year - account.Join_Date.Year) * 12) + now.Month - account.Join_Date.Month;

                //    // Hitung bulan sejak Limit_Updated_At terakhir
                //    var monthsSinceLastUpdate = ((now.Year - account.Limit_Updated_At?.Year) * 12) + now.Month - account.Limit_Updated_At?.Month;

                //    // Cari Title terkait
                //    var title = _context.Titles.FirstOrDefault(t => t.Id_Title == account.Id_Title);

                //    if (title == null) continue; // Jika Title tidak ditemukan, lewati akun ini

                //    // Reset tahunan (ulang tahun akun)
                //    bool isAnniversary = account.Join_Date.Day == now.Day && account.Join_Date.Month == now.Month && account.Join_Date.Year < now.Year;

                //    if (isAnniversary)
                //    {
                //        // Reset Current_Limit ke Reimburse_Limit
                //        account.Current_Limit = title.Reimburse_Limit;
                //    }
                //    else if (accountAgeInMonths < 12) // Akun dengan umur di bawah 1 tahun
                //    {
                //        // Tambahkan prorata untuk bulan-bulan yang belum diproses
                //        if (monthsSinceLastUpdate > 0)
                //        {
                //            float prorate = (float)(accountAgeInMonths / 12 * title.Reimburse_Limit);
                //            float prorateAmount = (float)(monthsSinceLastUpdate * (title.Reimburse_Limit / 12.0f));
                //            float total = (float)(prorateAmount + account.Current_Limit);

                //            if(total <= prorate)
                //            {
                //                account.Current_Limit = (float)Math.Round(total);
                //            }
                //        }
                //    }
                //    else // Akun dengan umur di atas atau sama dengan 1 tahun
                //    {
                //        if (monthsSinceLastUpdate >= 12)
                //        {
                //            account.Current_Limit = title.Reimburse_Limit;
                //        }
                //    }


            }



            // Simpan perubahan ke database
            await _context.SaveChangesAsync();
        }
    }
}
