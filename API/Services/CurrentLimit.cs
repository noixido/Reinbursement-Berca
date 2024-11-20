using API.Context;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CurrentLimit
    {
        private readonly MyContext _context;

        public CurrentLimit(MyContext context)
        {
            _context = context;
        }

        public async Task UpdateCurrentLimitsAsync()
        {
            var accounts = await _context.AccountDetails
                .Include(a => a.Title)
                .ToListAsync();

            foreach (var account in accounts)
            {
                var today = DateTime.UtcNow.Date; // Pastikan hanya tanggal tanpa waktu
                var joinDate = account.Join_Date;

                // Reset tahunan
                if (joinDate.Year < today.Year)
                {
                    // Cek apakah hari ini adalah ulang tahun join
                    if (joinDate.Day == today.Day && joinDate.Month == today.Month)
                    {
                        account.Current_Limit = account.Title.Reimburse_Limit;
                    }
                }

                // Prorata bulanan
                else if (joinDate.Day == today.Day)
                {
                    // Tambahkan prorata untuk satu bulan
                    account.Current_Limit += account.Title.Reimburse_Limit / 12.0f;
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
