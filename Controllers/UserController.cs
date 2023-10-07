using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using System;
using System.Threading.Tasks;
using App_Xamarin_Firebase.Models.UserModels;
using Firebase.Auth;

namespace App_Xamarin_Firebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FirebaseAdmin.Auth.FirebaseAuth _firebaseAuth;

        public UserController(FirebaseAdmin.Auth.FirebaseAuth firebaseAuth)
        {
            _firebaseAuth = firebaseAuth;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var provider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDbyX1XWzsDbnpKOwXRzZit3bsdqVj_7aU"));
                await provider.SendPasswordResetEmailAsync(model.Email);

                return Ok("Se ha enviado un correo electrónico para restablecer la contraseña.");
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("disable-account")]
        public async Task<IActionResult> DisableAccount(DisableAccountModel model)
        {
            try
            {
                var user = new UserRecordArgs
                {
                    Uid = model.UserId,
                    Disabled = true
                };

                await _firebaseAuth.UpdateUserAsync(user);

                return Ok("Cuenta deshabilitada exitosamente");
            }
            catch (FirebaseAuthException ex)
            {
                string errorMessage = ex.Message;
                return BadRequest(errorMessage);
            }
        }

        [HttpPost("delete-account")]
        public async Task<IActionResult> DeleteAccount(DeleteAccountModel model)
        {
            // Verificar que los datos del modelo sean válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _firebaseAuth.DeleteUserAsync(model.UserId);

                // La cuenta ha sido eliminada exitosamente
                return Ok("Cuenta eliminada");
            }
            catch (FirebaseAuthException ex)
            {
                // Manejar los errores de autenticación
                string errorMessage = ex.Message;
                return BadRequest(errorMessage);
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                string errorMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }
    }
}
