using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Title
    {
        [Key]
        public string? Id_Title { get; set; }
        public string Title_Name { get; set; }
        public float Reimburse_Limit { get; set; }

        [JsonIgnore]
        public ICollection<AccountDetail>? AccountDetails { get; set; }
    }
}
