using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.Tenancy;
using Glasswall.Kernel.Web.Authorisation;
using Glasswall.Tenant.Management.Tests.L0.MockData;
using Moq;
using NUnit.Framework;

namespace Glasswall.Tenant.Management.Tests.L0
{
    [TestFixture]
    [Category("TenantIdentityProvider")]
    internal class TenantIdentityProviderTests
    {
        [Test]
        [Category("TenantIdentityProvider")]
        public void When_instantiate_with_null_resolvers()
        {
            //ARRANGE
            var connectionStringProvider = new Mock<IConnectionStringProvider<NameValueCollection>>();
            var logger = new MockLogger<TenantIdentityProvider>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantIdentityProvider(connectionStringProvider.Object, null, logger));
        }

        [Test]
        [Category("TenantIdentityProvider")]
        public void When_instantiate_with_null_connection_string_provider_only()
        {
            //ARRANGE
            var logger = new MockLogger<TenantIdentityProvider>();
            var resolver = new Mock<ITenantResolver>();
            var resolvers = new[] { resolver.Object };
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantIdentityProvider(null, logger));
        }

        [Test]
        [Category("TenantIdentityProvider")]
        public void When_instantiate_with_null_connection_string_provider_only_and_logger_null()
        {
            //ARRANGE
            var logger = new MockLogger<TenantIdentityProvider>();
            var connectionStringProvider = new Mock<IConnectionStringProvider<NameValueCollection>>();
            var resolver = new Mock<ITenantResolver>();
            var resolvers = new[] { resolver.Object };
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantIdentityProvider(connectionStringProvider.Object, null));
        }

        [Test]
        [Category("TenantIdentityProvider")]
        public void When_instantiate_with_null_connection_string_provider_and_resolvers()
        {
            //ARRANGE
            var logger = new MockLogger<TenantIdentityProvider>();
            var resolver = new Mock<ITenantResolver>();
            var resolvers = new[] { resolver.Object };
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantIdentityProvider(null, resolvers, logger));
        }

        [Test]
        [Category("TenantIdentityProvider")]
        public async Task When_call_get_tenant_descriptor_with_resolvers()
        {
            //ARRANGE
            var tenantId = Guid.NewGuid();
            var logger = new MockLogger<TenantIdentityProvider>();
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var context = new TenantResolutionContext(new Kernel.Web.Endpoint("http://localhost"), bearerTokenContext.Object);
            context.Resolved(new TenantDescriptor(tenantId));
            var connectionStringProvider = new Mock<IConnectionStringProvider<NameValueCollection>>();
            connectionStringProvider.Setup(x => x.GetConnectionString(It.IsAny<TenantHttpContext>()))
                .Returns(new NameValueCollection());
            var resolver = new Mock<ITenantResolver>();
            
            resolver.Setup(x => x.ResolveTenant(It.IsAny<TenantResolutionContext>(), It.IsAny<Func<TenantResolutionContext, Task>>()))
                .Returns(Task.CompletedTask);
            
            var resolvers = new[] { resolver.Object };
            var tenantIdentityProvider = new TenantIdentityProvider(connectionStringProvider.Object, logger);

            //ACT
            var descriptor = await tenantIdentityProvider.GetTenantDescriptor(context, resolvers);
            
            //ASSERT
            
        }
    }
}