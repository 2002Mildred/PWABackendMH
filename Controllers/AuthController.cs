using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using SlackAPI;
using System.Security.Claims;
using Firebase.Auth;
using FirebaseConfig = Firebase.Auth.FirebaseConfig;
using App_Xamarin_Firebase.Models.AuthModels;
using FireSharp;

namespace App_Xamarin_Firebase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly FirebaseAdmin.Auth.FirebaseAuth auth;
        private readonly IFirebaseClient cliente;

        public AuthController(FirebaseAdmin.Auth.FirebaseAuth auth)
        {
            this.auth = auth;

            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = "yhMvDH9DX93xmp05w2EdKj01C7JOGH23cwWijeQ5",
                BasePath = "https://login-3b71d-default-rtdb.firebaseio.com/"
            };
            cliente = new FirebaseClient(config);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // Verificar que los datos del modelo sean válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string apiKey = "\r\nAIzaSyDbyX1XWzsDbnpKOwXRzZit3bsdqVj_7aU"; // Reemplaza esto con tu clave de API de Firebase

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

                var authResult = await authProvider.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

                string accessToken = authResult.FirebaseToken;
                string uuid = authResult.User.LocalId;

                // Devolver el token de acceso en la respuesta
                return Ok(new { AccessToken = accessToken, UUID = uuid });

            }
            catch (FirebaseAuthException ex)
            {
                // Manejar los errores de autenticación
                string errorMessage = ex.Message;
                return BadRequest(errorMessage);
            }
        }


       
        [HttpGet("test")]
        public IActionResult GetData()
        {
            // Obtén la identidad del usuario
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            // Obtén el ID del usuario (UID) del token
            var userId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Ahora puedes usar userId para obtener más información sobre el usuario si es necesario

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterModel model)
        {
            // Verificar que los datos del modelo sean válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Verificar si el correo electrónico ya está registrado
            var existingEmail = await GetUserInfoByEmail(model.Email);
            if (existingEmail != null)
            {
                return BadRequest("El correo electrónico ya está registrado");
            }
            // Verificar si el nombre de usuario ya existe
            var existingUsername = await GetUserInfoByUsername(model.Username);
            if (existingUsername != null)
            {
                return BadRequest("El nombre de usuario ya está ocupado");
            }
            try
            {
                // Crear el usuario en Firebase Authentication
                var userRecord = await auth.CreateUserAsync(new UserRecordArgs
                {
                    Email = model.Email,
                    Password = model.Password,
                });

                // Obtener el token de acceso del usuario registrado
                string accessToken = await auth.CreateCustomTokenAsync(userRecord.Uid);

                // Guardar la información adicional en la base de datos
                UserInfoModel userInfo = new UserInfoModel
                {
                    UserId = userRecord.Uid,
                    Email = model.Email,
                    Username = model.Username,
                    Gender = model.Gender,
                    Birthday = model.Birthday
                };
                string userId = userRecord.Uid;
                SetResponse response = cliente.Set("userInfo/" + userId, userInfo);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Redirigir o retornar una respuesta exitosa
                    return Ok("Registro exitoso");
                }
                else
                {
                    // Manejar el error en caso de fallo al guardar en la base de datos
                    return BadRequest("Error al guardar la información adicional");
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Manejar los errores de autenticación
                string errorMessage = ex.Message;
                return BadRequest(errorMessage);
            }
        }
        private async Task<UserInfoModel> GetUserInfoByUsername(string username)
        {
            FirebaseResponse response = await cliente.GetAsync("userInfo");
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string, UserInfoModel> users = response.ResultAs<Dictionary<string, UserInfoModel>>();
                if (users != null)
                {
                    foreach (var user in users.Values)
                    {
                        if (user.Username == username)
                        {
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        private async Task<UserInfoModel> GetUserInfoByEmail(string email)
        {
            FirebaseResponse response = await cliente.GetAsync("userInfo");
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string, UserInfoModel> users = response.ResultAs<Dictionary<string, UserInfoModel>>();
                if (users != null)
                {
                    return users.Values.FirstOrDefault(u => u.Email == email);
                }
            }
            return null;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                FirebaseResponse response = await cliente.GetAsync($"userInfo/{userId}");
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UserInfoModel userInfo = response.ResultAs<UserInfoModel>();
                    if (userInfo != null)
                    {
                        return Ok(userInfo); // Devuelve la información del usuario encontrado
                    }
                }

                return NotFound("Usuario no encontrado");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }



        [HttpPost("add-imc")]
        public async Task<IActionResult> AddIMCToUser([FromBody] IMCModel model)
        {
            try
            {
                // Verificar que el usuario exista en Firebase
                FirebaseResponse userInfoResponse = await cliente.GetAsync($"userInfo/{model.UserId}");

                if (userInfoResponse != null && userInfoResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UserInfoModel userInfo = userInfoResponse.ResultAs<UserInfoModel>();

                    // Actualizar el IMC en la información del usuario
                    userInfo.IMC = model.IMC;

            
                    SetResponse response = cliente.Set($"userInfo/{ model.UserId}", userInfo);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Ok("IMC agregado con éxito");
                    }
                }

                return NotFound("Usuario no encontrado");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }

    }
}