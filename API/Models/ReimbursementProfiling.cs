using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class ReimbursementProfiling
    {
        [Column(Order = 0)]
        [ForeignKey("Account")]
        public string? Id_Account { get; set; }

        [Column(Order = 1)]
        [ForeignKey("Reimbursement")]
        public string? Id_Reimbursement { get; set; }

        public Account? Account { get; set; }
        public Reimbursement? Reimbursement { get; set; }
    }
}
