using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class AccountDetail
    {
        [Key]
        public string? Id_AccountDetail { get; set; }
        [ForeignKey("Account")]
        public string? Id_Account { get; set; }
        [ForeignKey("Title")]
        public string? Id_Title { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime Birth_Date { get; set; }
        public float? Current_Limit { get; set; }
        public DateTime Join_Date { get; set; }

        public Account? Account { get; set; }
        public Title? Title { get; set; }
    }
}
