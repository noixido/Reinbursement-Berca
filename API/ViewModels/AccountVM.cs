using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AccountVM
    {
        public string Email { get; set; }
        public string Role_Name { get; set; }
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
        public int Current_Limit { get; set; }
    }

    public class ShowAccountVM
    {
        public string Id_Account { get; set; }
        public string Email { get; set; }
        public string Role_Name { get; set; }
        public string Title_Name { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Birth_Date { get; set; }
        public string Join_Date { get; set; }
        public int Current_Limit { get; set; }
    }
}
