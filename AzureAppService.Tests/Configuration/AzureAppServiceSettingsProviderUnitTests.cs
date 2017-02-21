using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Azure.AppService.Core.Configuration
{
    public class AzureAppServiceSettingsProviderUnitTests
    {
        [Fact]
        public void Constructor()
        {
            var env = new Dictionary<string, string>();
            var provider = new AzureAppServiceSettingsProvider(env);
            Assert.NotNull(provider);
        }

        [Fact]
        public void CreatesConnectionStrings()
        {
            var env = new Dictionary<string, string>()
            {
                { "SQLCONNSTR_MS_TableConnectionString", "test1" },
                { "SQLAZURECONNSTR_DefaultConnection", "test2" },
                { "SQLCONNSTRMSTableConnectionString", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("ConnectionStrings:MS_TableConnectionString", out r));
            Assert.Equal("test1", r);

            Assert.True(provider.TryGet("ConnectionStrings:DefaultConnection", out r));
            Assert.Equal("test2", r);

            Assert.False(provider.TryGet("ConnectionStrings:MSTableConnectionString", out r));
        }

        [Fact]
        public void CreatesDataConnections()
        {
            var env = new Dictionary<string, string>()
            {
                { "SQLCONNSTR_MS_TableConnectionString", "test1" },
                { "SQLAZURECONNSTR_DefaultConnection", "test2" },
                { "SQLCONNSTRMSTableConnectionString", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("Data:MS_TableConnectionString:Type", out r));
            Assert.Equal("SQL", r);
            Assert.True(provider.TryGet("Data:MS_TableConnectionString:ConnectionString", out r));
            Assert.Equal("test1", r);

            Assert.True(provider.TryGet("Data:DefaultConnection:Type", out r));
            Assert.Equal("SQLAZURE", r);
            Assert.True(provider.TryGet("Data:DefaultConnection:ConnectionString", out r));
            Assert.Equal("test2", r);

            Assert.False(provider.TryGet("Data:MSTableConnectionString:Type", out r));
            Assert.False(provider.TryGet("Data:MSTableConnectionString:ConnectionString", out r));
        }

        [Fact]
        public void HandlesPushConnectionString()
        {
            var env = new Dictionary<string, string>()
            {
                { "CUSTOMCONNSTR_MS_NotificationHubConnectionString", "test1" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.False(provider.TryGet("Data:MS_NotificationHubConnectionString:Type", out r));
            Assert.False(provider.TryGet("Data:MS_NotificationHubConnectionString:ConnectionString", out r));
            Assert.True(provider.TryGet("ConnectionStrings:MS_NotificationHubConnectionString", out r));
            Assert.Equal("test1", r);
        }

        [Fact]
        public void HandlesExpressAADSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_AUTO_AAD", "True" },
                { "WEBSITE_AUTH_CLIENT_ID", "test2" },
                { "WEBSITE_AUTH_OPENID_ISSUER", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:Mode", out r));
            Assert.Equal("Express", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:ClientId", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:Issuer", out r));
            Assert.Equal("test3", r);
        }

        [Fact]
        public void HandlesAdvancedAADSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_AUTO_AAD", "False" },
                { "WEBSITE_AUTH_CLIENT_ID", "test1" },
                { "WEBSITE_AUTH_CLIENT_SECRET", "test2" },
                { "WEBSITE_AUTH_OPENID_ISSUER", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:Mode", out r));
            Assert.Equal("Advanced", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:ClientId", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:ClientSecret", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:AzureActiveDirectory:Issuer", out r));
            Assert.Equal("test3", r);
        }

        [Fact]
        public void HandlesFacebookSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_FB_APP_ID", "test1" },
                { "WEBSITE_AUTH_FB_APP_SECRET", "test2" },
                { "WEBSITE_AUTH_FB_SCOPE", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:Facebook:ClientId", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:Facebook:ClientSecret", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:Facebook:SCOPE", out r));
            Assert.Equal("test3", r);
        }

        [Fact]
        public void HandlesGoogleSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_GOOGLE_CLIENT_ID", "test1" },
                { "WEBSITE_AUTH_GOOGLE_CLIENT_SECRET", "test2" },
                { "WEBSITE_AUTH_GOOGLE_SCOPE", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:Google:ClientId", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:Google:ClientSecret", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:Google:SCOPE", out r));
            Assert.Equal("test3", r);
        }

        [Fact]
        public void HandlesMSASettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_MSA_CLIENT_ID", "test1" },
                { "WEBSITE_AUTH_MSA_CLIENT_SECRET", "test2" },
                { "WEBSITE_AUTH_MSA_SCOPE", "test3" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:MicrosoftAccount:ClientId", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:MicrosoftAccount:ClientSecret", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:MicrosoftAccount:SCOPE", out r));
            Assert.Equal("test3", r);
        }

        [Fact]
        public void HandlesTwitterSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_TWITTER_CONSUMER_KEY", "test1" },
                { "WEBSITE_AUTH_TWITTER_CONSUMER_SECRET", "test2" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:Twitter:ClientId", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:Twitter:ClientSecret", out r));
            Assert.Equal("test2", r);
        }

        [Fact]
        public void HandlesAuthSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_AUTH_SIGNING_KEY", "test1" },
                { "WEBSITE_AUTH_ALLOWED_AUDIENCES", "test2" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Auth:SigningKey", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Auth:ALLOWED_AUDIENCES", out r));
            Assert.Equal("test2", r);
        }

        [Fact]
        public void HandlesPushSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "CUSTOMCONNSTR_MS_NotificationHubConnectionString", "test1" },
                { "WEBSITE_PUSH_TAGS", "test2" },
                { "WEBSITE_PUSH_ENABLED", "True" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Push:ConnectionString", out r));
            Assert.Equal("test1", r);
            Assert.True(provider.TryGet("AzureAppService:Push:TAGS", out r));
            Assert.Equal("test2", r);
            Assert.True(provider.TryGet("AzureAppService:Push:ENABLED", out r));
            Assert.Equal("True", r);
        }

        [Fact]
        public void HandlesWebsiteSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "WEBSITE_HOST_NAME", "test1" },
                { "WEBSITE_PUSH_TAGS", "test2" },
                { "WEBSITE_AUTH_ENABLED", "True" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:Website:HOST_NAME", out r));
            Assert.Equal("test1", r);
            Assert.False(provider.TryGet("AzureAppService:Website:PUSH_TAGS", out r));
            Assert.False(provider.TryGet("AzureAppService:Website:AUTH_ENABLED", out r));
        }

        [Fact]
        public void HandlesAppsettingSettings()
        {
            var env = new Dictionary<string, string>()
            {
                { "APPSETTING_FOO", "test1" },
                { "APPSETTING_WEBSITE_HOST_NAME", "test2" },
                { "APPSETTING_WEBSITE_PUSH_TAGS", "test3" },
                { "APPSETTING_WEBSITE_AUTH_ENABLED", "True" }
            };
            var provider = new AzureAppServiceSettingsProvider(env);
            provider.Load();

            string r;
            Assert.True(provider.TryGet("AzureAppService:AppSetting:FOO", out r));
            Assert.Equal("test1", r);
            Assert.False(provider.TryGet("AzureAppService:AppSetting:WEBSITE_PUSH_TAGS", out r));
            Assert.False(provider.TryGet("AzureAppService:AppSetting:WEBSITE_AUTH_ENABLED", out r));
            Assert.False(provider.TryGet("AzureAppService:AppSetting:PUSH_TAGS", out r));
            Assert.False(provider.TryGet("AzureAppService:AppSetting:AUTH_ENABLED", out r));
        }
    }
}
