using System;
using System.ComponentModel.DataAnnotations;

namespace App_Xamarin_Firebase.Models.AuthModels
{
    public class UserInfoModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
    }
}
