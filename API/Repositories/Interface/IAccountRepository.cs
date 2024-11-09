using API.ViewModels;

namespace API.Repositories.Interface
{
    public interface IAccountRepository
    {
        // CRUD
        int AddAccount(AccountVM accountVM); // tambah data
        IEnumerable<ShowAccountVM> GetAllAccounts(); // lihat semua data
        ShowAccountVM GetAccountByEmail(string email); // lihat data berdasarkan email
        int UpdateAccount(AccountVM accountVM); // perbaharui data berdasarkan email
        int DeleteAccount(string email); // hapus data berdasarkan email

        // Lain-lain
        string GenerateCustomIdForAccount(); // generate custom id untuk tabel Account
        AccountVM GetLastInsertedAccount(); // ambil data terakhir yang disimpan
        int ChangePassword(ChangePasswordVM changePasswordVM);
    }
}
