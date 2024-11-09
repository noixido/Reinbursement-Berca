using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class AccountDetail
    {
        [Key]
        [ForeignKey("Account")]
        public string Id_AccountDetail { get; set; }
        [ForeignKey("Title")]
        public string Id_Title { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birth_Date { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Join_Date { get; set; }
        public float Current_Limit { get; set; }

        public virtual Account Account { get; set; }
        public virtual Title Title { get; set; }
    }
}
