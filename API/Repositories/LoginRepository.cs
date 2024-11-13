using API.Context;
using API.Repositories.Interface;
using API.ViewModels;

namespace API.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly MyContext _context;
        public LoginRepository(MyContext context)
        {
            _context = context;
        }

        public PayloadVM GetPayload(string email)
        {
            var account = _context.Accounts
                .Where(x => x.Email == email)
                .FirstOrDefault();
            
            if(account == null)
            {
                throw new Exception("Data not found!");
            }

            var accountDetail = _context.AccountDetails
                .Where(id => id.Id_AccountDetail == account.Id_Account)
                .FirstOrDefault();
            return new PayloadVM
            {
                Id_Account = account.Id_Account,
                Email = email,
                Name = accountDetail.Name,
                Roles = account.Role_Name
            };
        }

        public bool Login(LoginVM loginVM)
        {
            var account = _context.Accounts
                .Where(ie => ie.AccountDetails.IsEmployee == 1)
                .FirstOrDefault(e => e.Email == loginVM.Email);
            if (account == null)
            {
                throw new Exception("Email is invalid!");
            }

            var checkPass = BCrypt.Net.BCrypt.Verify(loginVM.Password, account.Password);
            if(!checkPass)
            {
                throw new Exception("Password is incorrect!");
            }
            return true;
        }
    }
}
