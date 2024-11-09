using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Account
    {
        [Key]
        public string Id_Account { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role_Name { get; set; }

        public virtual AccountDetail AccountDetails { get; set; }
        public ICollection<ReimbursementProfiling> ReimbursementProfilings { get; set; }
    }
}
