using System.ComponentModel.DataAnnotations;

namespace AuthrisationApi.Models
{
    public class BaseModel
    {
        [Key]
        public string Id { get; set; }
    }
}