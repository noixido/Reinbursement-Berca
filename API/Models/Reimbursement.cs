using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Reimbursement
    {
        [Key]
        public string Id_Reimbursement { get; set; }
        [ForeignKey("Category")]
        public string Id_Category { get; set; }

        public string Evidence { get; set; }
        public float Amount { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; }
        public DateTime Submit_Date { get; set; }

        public Category Category { get; set; }
        public ICollection<ReimbursementProfiling> ReimbursementProfilings { get; set; }
    }
}
