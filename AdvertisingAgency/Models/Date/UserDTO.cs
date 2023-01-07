using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvertisingAgency.Models.Date
{
    [Table("TableUsers")]
    public class UserDTO
    {
         [Key]
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string EmailAdress { get; set; }
         public string Username { get; set; }
         public string Password { get; set; }
    }
}