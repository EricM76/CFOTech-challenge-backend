using System.ComponentModel.DataAnnotations;

namespace ChallengeCFOTech.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
    }
}
