using System.ComponentModel.DataAnnotations;

namespace App_Xamarin_Firebase.Models.UserModels
{
    public class DeleteAccountModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
