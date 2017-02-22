using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.Azure.AppService.Core.Authentication
{
    /// <summary>
    /// Options class provides information needed to control Azure App Service
    /// Authentication middleware behavior.
    /// </summary>
    public class AzureAppServiceAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Creates an instance of AzureAppServiceAuthenticationOptions with
        /// default values.
        /// </summary>
        public AzureAppServiceAuthenticationOptions() : base()
        {
            AuthenticationScheme = AzureAppServiceAuthenticationDefault.AuthenticationScheme;
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;

            var signingKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            if (signingKey != null)
            {
                SigningKey = signingKey;
            }

            var website = Environment.GetEnvironmentVariable("WEBSITE_HOST_NAME");
            var allowedAudiences = Environment.GetEnvironmentVariable("WEBSITE_AUTH_ALLOWED_AUDIENCES");
            if (allowedAudiences != null)
            {
                AllowedAudiences = allowedAudiences.Split(' ');
            }
            else if (website != null)
            {
                AllowedAudiences = new string[] { $"https://{website}/" }; 
            }

            var allowedIssuers = Environment.GetEnvironmentVariable("WEBSITE_AUTH_ALLOWED_ISSUERS");
            if (allowedIssuers != null)
            {
                AllowedIssuers = allowedIssuers.Split(' ');
            }
            else if (website != null)
            {
                AllowedIssuers = new string[] { $"https://{website}/" };
            }
        }

        /// <summary>
        /// Getter/Setter for the enabled flag, which indicates if we should even
        /// do authentication.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Signing Key used to sign the inbound JWT Bearer token
        /// </summary>
        public string SigningKey { get; set; }

        /// <summary>
        /// List of allowed audiences in the JWT Bearer token
        /// </summary>
        public string[] AllowedAudiences { get; set; }

        /// <summary>
        /// List of allowed issuers in the JWT Bearer token
        /// </summary>
        public string[] AllowedIssuers { get; set; }
    }
}
