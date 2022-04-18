using System;
using System.Threading.Tasks;
using Glasswall.FileTrust.Tenant.Management.Tests.L0.MockData;
using Glasswall.Kernel.Data.Tenancy;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Tenancy;
using Glasswall.Kernel.Web.Authorisation;
using Glasswall.Tenant.Management;
using Moq;
using NUnit.Framework;

namespace Glasswall.FileTrust.Tenant.Management.Tests.L0
{
    [TestFixture]
    public class TenantManagerTests
    {
        [Test]
        [Category("TenantManager")]
        public void TenantManager_null_tenantIdentityProvider_test()
        {
            //ARRANGE
            var filterBuilder = new Mock<ITenantFilterBuilder>();
            var logger = new Mock<IGWLogger<TenantManager>>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantManager(null, logger.Object));
        }
        
        [Test]
        [Category("TenantManager")]
        public void TenantManager_null_logger_test()
        {
            //ARRANGE
            var resolversFactory = new Mock<ITenantIdentityProvider>();
            var logger = new Mock<IGWLogger<TenantManager>>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantManager(resolversFactory.Object, null));
        }
        
        [Test]
        [Category("TenantManager")]
        public async Task When_calls_get_tenant_descriptor_tenant_edentity_provider_get_tenant_descriptor_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tenantModel = new TenantMockModel(tenantId);
            
            var descriptorProvider = new Mock<ITenantIdentityProvider>();
            descriptorProvider.Setup(x => x.GetTenantDescriptor(It.IsAny<TenantResolutionContext>()))
                .ReturnsAsync(new TenantDescriptor(tenantId));
            var logger = new Mock<IGWLogger<TenantManager>>();
            var tenantManager = new TenantManager(descriptorProvider.Object, logger.Object);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var context = new TenantResolutionContext(new Kernel.Web.Endpoint("http://localhost"), bearerTokenContext.Object);
            //ACT
            await tenantManager.GetTenantDescriptor(context);

            //ASSERT
            descriptorProvider.Verify(x => x.GetTenantDescriptor(It.IsAny<TenantResolutionContext>()), Times.Once);
        }
    }
}