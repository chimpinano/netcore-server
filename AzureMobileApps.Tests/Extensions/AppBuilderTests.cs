using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Mobile.Core.Server.Extensions;
using Moq;
using System;
using Xunit;

namespace AzureMobileApps.Tests.Extensions
{
    public class AppBuilderTests
    {
        [Fact]
        public void UseAzureMobileApps_Throws_ArgumentNullException()
        {
            var applicationBuilderMock = new Mock<IApplicationBuilder>();
            applicationBuilderMock
                .Setup(s => s.ApplicationServices)
                .Returns(Mock.Of<IServiceProvider>());

            Assert.Throws<ArgumentNullException>(() => applicationBuilderMock.Object.UseAzureMobileApps(null));
        }

        [Fact]
        public void UseAzureMobileApps_Throws_ArgumentException()
        {
            var applicationBuilderMock = new Mock<IApplicationBuilder>();
            applicationBuilderMock
                .Setup(s => s.ApplicationServices)
                .Returns(Mock.Of<IServiceProvider>());

            Assert.Throws<ArgumentException>(() => applicationBuilderMock.Object.UseAzureMobileApps(t => { }));
        }
    }
}
