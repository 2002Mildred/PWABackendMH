using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App_Xamarin_Firebase.Authentication
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
    {
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseAuthenticationHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FirebaseAuth firebaseAuth)
            : base(options, logger, encoder, clock)
        {
            _firebaseAuth = firebaseAuth;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            string bearerToken = Context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(bearerToken) || !bearerToken.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Invalid Scheme");
            }

            string token = bearerToken.Substring("Bearer ".Length);

            try
            {
                FirebaseToken decodedToken = await _firebaseAuth.VerifyIdTokenAsync(token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    // Agrega otras reclamaciones según tus necesidades
                };

                var identity = new ClaimsIdentity(claims, nameof(FirebaseAuthenticationHandler));
                var principal = new ClaimsPrincipal(identity);
                var authenticationTicket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(authenticationTicket);
            }
            catch (FirebaseAuthException)
            {
                return AuthenticateResult.Fail("Invalid Token");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while verifying Firebase token: {ErrorMessage}", ex.Message);
                return AuthenticateResult.Fail(ex);
            }
        }
    }
}
