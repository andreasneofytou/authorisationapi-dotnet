using System.ComponentModel.DataAnnotations;

namespace AuthorisationApi.Models
{
    public class BaseModel
    {
        [Key]
        public string Id { get; set; }
    }
}