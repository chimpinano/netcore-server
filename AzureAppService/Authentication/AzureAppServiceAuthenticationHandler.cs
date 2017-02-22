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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Principal;

namespace Microsoft.Azure.AppService.Core.Authentication
{
    internal class AzureAppServiceAuthenticationHandler : AuthenticationHandler<AzureAppServiceAuthenticationOptions>
    {
        private HttpClient client = new HttpClient();

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
            // If the signingKey is the signature
            signingKeys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SigningKey)));
            // If it's base-64 encoded
            try
            {
                signingKeys.Add(new SymmetricSecurityKey(Convert.FromBase64String(Options.SigningKey)));
            } catch (FormatException) { /* The key was not base 64 */ }
            // If it's hex encoded, then decode the hex and add it
            try
            {
                if (Options.SigningKey.Length % 2 == 0)
                {
                    signingKeys.Add(new SymmetricSecurityKey(
                        Enumerable.Range(0, Options.SigningKey.Length)
                                  .Where(x => x % 2 == 0)
                                  .Select(x => Convert.ToByte(Options.SigningKey.Substring(x, 2), 16))
                                  .ToArray()
                    ));
                }
            } catch (Exception) {  /* The key was not hex-encoded */ }

            // validation parameters
            var websiteAuthEnabled = Environment.GetEnvironmentVariable("WEBSITE_AUTH_ENABLED");
            var inAzureAppService = (websiteAuthEnabled != null && websiteAuthEnabled.Equals("True", StringComparison.OrdinalIgnoreCase));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signature must have been created by the signing key
                ValidateIssuerSigningKey = !inAzureAppService,
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

            // Get the actual claims from the {issuer}/.auth/me
            try
            {
                client.BaseAddress = new Uri(validatedToken.Issuer);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", token);

                HttpResponseMessage response = await client.GetAsync("/.auth/me");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var userRecord = JsonConvert.DeserializeObject<List<AzureAppServiceClaims>>(jsonContent).First();

                    // Create a new ClaimsPrincipal based on the results of /.auth/me
                    List<Claim> claims = new List<Claim>();
                    foreach (var claim in userRecord.UserClaims)
                    {
                        claims.Add(new Claim(claim.Type, claim.Value));
                    }
                    claims.Add(new Claim("x-auth-provider-name", userRecord.ProviderName));
                    claims.Add(new Claim("x-auth-provider-token", userRecord.IdToken));
                    claims.Add(new Claim("x-user-id", userRecord.UserId));
                    var identity = new GenericIdentity(principal.Claims.Where(x => x.Type.Equals("stable_sid")).First().Value, Options.AuthenticationScheme);
                    identity.AddClaims(claims);
                    principal = new ClaimsPrincipal(identity);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return AuthenticateResult.Fail("/.auth/me says you are unauthorized");
                }
                else
                {
                    Logger.LogWarning($"/.auth/me returned status = {response.StatusCode} - skipping user claims population");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Unable to get /.auth/me user claims - skipping (ex = {ex.GetType().FullName}, msg = {ex.Message})");
            }

            // Generate a new authentication ticket and return success
            var ticket = new AuthenticationTicket(
                principal, new AuthenticationProperties(),
                Options.AuthenticationScheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}