using System;
using Microsoft.Azure.AppService.Core.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add Azure App Service Authentication
    /// capabilities to an HTTP application pipeline
    /// </summary>
    public static class AppServiceAuthenticationAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="AzureAppServiceAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables authorization of the JWT-based Azure App 
        /// Service authentication.  
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseAzureAppServiceAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<AzureAppServiceAuthenticationMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="AzureAppServiceAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables authorization of the JWT-based Azure App 
        /// Service authentication.  
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">The <see cref="AzureAppServiceAuthenticationOptions"/> that specified options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseAzureAppServiceAuthentication(this IApplicationBuilder app, AzureAppServiceAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return app.UseMiddleware<AzureAppServiceAuthenticationMiddleware>(Options.Create(options));
        }
    }
}
