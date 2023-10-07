using System.ComponentModel.DataAnnotations;

namespace App_Xamarin_Firebase.Models.UserModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string Email { get; set; }
    }
}
