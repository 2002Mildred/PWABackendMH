using System;
using System.ComponentModel.DataAnnotations;

namespace App_Xamarin_Firebase.Models.AuthModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "El campo Email es requerido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo Password es requerido.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "El campo Username es requerido.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "El campo Gender es requerido.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "El campo Birthday es requerido.")]
        public DateTime Birthday { get; set; }
    }
}
