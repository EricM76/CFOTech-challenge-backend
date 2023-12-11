using System.ComponentModel.DataAnnotations;

namespace ChallengeCFOTech.Models.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "La contraseña de usuario es obligatoria")]
        public string Password { get; set; }
    }
}
