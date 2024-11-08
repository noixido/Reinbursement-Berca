using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Category
    {
        [Key]
        public string? Id_Category { get; set; }
        public string Category_Name { get; set; }

        public ICollection<Reimbursement>? Reimbursements { get; set; }
    }
}
