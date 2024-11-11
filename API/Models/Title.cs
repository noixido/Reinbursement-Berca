using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Title
    {
        [Key]
        public string Id_Title { get; set; }
        public string Title_Name { get; set; }
        public float Reimburse_Limit { get; set; }

<<<<<<< HEAD
=======
        [JsonIgnore]
>>>>>>> 44d286580ab68d5dcf022f2c1abca43f923cc16f
        public ICollection<AccountDetail>? AccountDetails { get; set; }
    }
}
