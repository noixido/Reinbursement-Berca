using System.ComponentModel.DataAnnotations.Schema;

namespace API.ViewModels
{
    public class ReimbursementVM
    {
        public string? Id_Account { get; set; }
        public string? Name { get; set; }
        public string? Id_Reimbursement { get; set; }
        public string? Id_Category { get; set; }
        public string? Evidence { get; set; }
        public float? Amount { get; set; }
        public float? Approve_Amount { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }
        public DateTime? Submit_Date { get; set; }
        public string? Category_Name { get; set; }
    }

    public class ChangeStatusReimbursement
    {
        public float? Approve_Amount { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
    }
}
