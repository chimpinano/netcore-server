using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using System.Collections.Generic;

namespace Microsoft.Azure.AppService.Core.Authentication
{
    internal class AzureAppServiceAuthenticationHandler : AuthenticationHandler<AzureAppServiceAuthenticationOptions>
    {
        /// <summary>
        /// Searches for the 'X-ZUMO-AUTH' header for a token.  If the tokne is found, it is validated using
        /// the options in the <see cref="AzureAppServiceAuthenticationOptions"/>.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Grab the X-ZUMO-AUTH token if it is available
            // If not, then try the Authorization Bearer token
            string token = Request.Headers["X-ZUMO-AUTH"];
            if (string.IsNullOrEmpty(token))
            {
                string authorization = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.Skip();
                }
                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                    if (string.IsNullOrEmpty(token))
                    {
                        return AuthenticateResult.Skip();
                    }
                }
            }
            Logger.LogDebug($"Obtained Authorization Token = {token}");

            // Convert the signing key we have to something we can use
            var signingKeys = new List<SecurityKey>();
            signingKeys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SigningKey)));
            // The signing key may also be base-64 encoded
            try
            {
                byte[] b64key = Convert.FromBase64String(Options.SigningKey);
                signingKeys.Add(new SymmetricSecurityKey(b64key));
            } catch (Exception) { /* Ignore this error - it's the base-64 decoder */ }

            // validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signature must have been created by the signing key
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,

                // The Issuer (iss) claim must match
                ValidateIssuer = true,
                ValidIssuers = Options.AllowedIssuers,

                // The Audience (aud) claim must match
                ValidateAudience = true,
                ValidAudiences = Options.AllowedAudiences,

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow clock drift, set that here
                ClockSkew = TimeSpan.FromSeconds(60)
            };

            // validate the token we received
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(101, ex, "Cannot validate JWT");
                return AuthenticateResult.Fail(ex);
            }

            // Generate a new authentication ticket and return success
            var ticket = new AuthenticationTicket(
                principal, new AuthenticationProperties(),
                Options.AuthenticationScheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}