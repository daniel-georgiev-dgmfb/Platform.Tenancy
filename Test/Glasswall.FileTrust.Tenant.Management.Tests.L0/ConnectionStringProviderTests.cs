using Moq;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Platform.Tenant.Management.Tests.L0
{
	[TestFixture]
    internal class ConnectionStringProviderTests
    {
        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_null_connection_string_retriever_test()
        {
            //ARRANGE
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new Mock<ICacheProvider>();
            var logger = new MockLogger<TenantAttributesProvider>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesProvider(null, cache.Object, secretManager.Object, logger));
        }
        
        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_null_cache_test()
        {
            //ARRANGE
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new Mock<ICacheProvider>();
            var logger = new MockLogger<TenantAttributesProvider>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesProvider(tenantConnectionStringRetriever.Object, null, secretManager.Object, logger));
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_null_secret_manager_test()
        {
            //ARRANGE
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new Mock<ICacheProvider>();
            var logger = new MockLogger<TenantAttributesProvider>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache.Object, null, logger));
        }
        
        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_null_logger_test()
        {
            //ARRANGE
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new Mock<ICacheProvider>();
            var logger = new MockLogger<TenantAttributesProvider>();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache.Object, secretManager.Object, null));
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_string_test_tenant_http_context_null()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new MockCacheProvider();
            
            var logger = new MockLogger<TenantAttributesProvider>();
            
            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);

            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => connectionStringProvider.GetConnectionString((TenantHttpContext)null));
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_string_test_context_is_not_tenant_http_context_()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new MockCacheProvider();

            var logger = new MockLogger<TenantAttributesProvider>();
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);
            var context = new TenantResolutionContext(new Endpoint("http://localhost"), bearerTokenContext.Object);
            //ACT

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => connectionStringProvider.GetConnectionString(context));
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_string_without_parameter_throws_not_implemented_exception()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            var secretManager = new Mock<ISecretManager>();
            var cache = new MockCacheProvider();

            var logger = new MockLogger<TenantAttributesProvider>();
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);
            
            //ACT

            //ASSERT
            Assert.Throws<NotImplementedException>(() => connectionStringProvider.GetConnectionString());
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_string_test_get_connection_string_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, bearerTokenContext.Object);
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            tenantConnectionStringRetriever.Setup(x => x.GetTenantAttributes(It.Is<TenantHttpContext>(c => c == tenantContext), CancellationToken.None))
                .ReturnsAsync(new TenantModel { TenantName = "TestTenant", DisplayName = "Test tenant", ConnectionStringAttributes = new ConnectionStringAttributes { Server = "server", Database = "database", User = "userName", SecretName = "secret" } })
                .Verifiable();
            
            var secretManager = new Mock<ISecretManager>();

            var cache = new MockCacheProvider();
           
            var logger = new MockLogger<TenantAttributesProvider>();

            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);

            //ACT
            connectionStringProvider.GetConnectionString(tenantContext);
            //ASSERT
            tenantConnectionStringRetriever.Verify();
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_string_test_get_secret_string_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, bearerTokenContext.Object);
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            tenantConnectionStringRetriever.Setup(x => x.GetTenantAttributes(It.Is<TenantHttpContext>(c => c == tenantContext), CancellationToken.None))
                .ReturnsAsync(new TenantModel { TenantName = "TestTenant", DisplayName = "Test tenant", ConnectionStringAttributes = new ConnectionStringAttributes { Server = "server", Database = "database", User = "userName", SecretName = "secret" } });


            var secretManager = new Mock<ISecretManager>();
            secretManager.Setup(x => x.GetSecret(It.Is<SecretContext>(s => s.SecretName == "secret")))
                .ReturnsAsync("secretValue")
                .Verifiable();
            var cache = new MockCacheProvider();
            var logger = new MockLogger<TenantAttributesProvider>();

            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);

            //ACT
            connectionStringProvider.GetConnectionString(tenantContext);
            //ASSERT
            secretManager.Verify();
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_returns_expected_collection()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, bearerTokenContext.Object);
            var expectedNameCollection = new NameValueCollection
                {
                    { "DataSource", "server" },
                    { "DataBase", "database"},
                    {"TenantName", "TestTenant" },
                    { "UserName", "userName"},
                    { "Password", "secretValue" }
                };
            
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            tenantConnectionStringRetriever.Setup(x => x.GetTenantAttributes(It.Is<TenantHttpContext>(c => c == tenantContext), CancellationToken.None))
                .ReturnsAsync(new TenantModel { TenantName = "TestTenant", DisplayName = "Test tenant", ConnectionStringAttributes = new ConnectionStringAttributes { Server = "server", Database = "database", User = "userName", SecretName = "secret" } });
            var secretManager = new Mock<ISecretManager>();
            secretManager.Setup(x => x.GetSecret(It.Is<SecretContext>(s => s.SecretName == "secret")))
                .ReturnsAsync("secretValue");
            var cache = new MockCacheProvider();
            var logger = new MockLogger<TenantAttributesProvider>();

            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);

            //ACT
            var nameCollection = connectionStringProvider.GetConnectionString(tenantContext);
            
            //ASSERT
            Assert.AreEqual(expectedNameCollection.AllKeys.OrderBy(x => x), nameCollection.AllKeys.OrderBy(x => x));
            Assert.AreEqual(expectedNameCollection.GetValues(0), nameCollection.GetValues(expectedNameCollection.AllKeys[0]));
            Assert.AreEqual(expectedNameCollection.GetValues(1), nameCollection.GetValues(expectedNameCollection.AllKeys[1]));
            Assert.AreEqual(expectedNameCollection.GetValues(2), nameCollection.GetValues(expectedNameCollection.AllKeys[2]));
            Assert.AreEqual(expectedNameCollection.GetValues(3), nameCollection.GetValues(expectedNameCollection.AllKeys[3]));
        }

        [Test]
        [Category("ConnectionStringProvider")]
        public void ConnectionStringProvider_get_connection_returns_expected_collection_from_cache()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, bearerTokenContext.Object);
            var expectedNameCollection = new NameValueCollection
                {
                    { "DataSource", "server" },
                    { "DataBase", "database"},
                    { "UserName", "userName"},
                    { "Password", "secretValue" }
                };
            
            var tenantConnectionStringRetriever = new Mock<ITenantAttributesRetriever>();
            tenantConnectionStringRetriever.Setup(x => x.GetTenantAttributes(It.Is<TenantHttpContext>(c => c == tenantContext), CancellationToken.None))
                .ReturnsAsync(new TenantModel { TenantName = "TestTenant", DisplayName = "Test tenant", ConnectionStringAttributes = new ConnectionStringAttributes { Server = "server", Database = "database", User = "userName", SecretName = "secret" } })
                .Verifiable();
            var secretManager = new Mock<ISecretManager>();
            secretManager.Setup(x => x.GetSecret(It.Is<SecretContext>(s => s.SecretName == "secret")))
                .ReturnsAsync("secretValue")
                .Verifiable();
            var cache = new MockCacheProvider(tenantId, expectedNameCollection);
            var logger = new MockLogger<TenantAttributesProvider>();

            var connectionStringProvider = new TenantAttributesProvider(tenantConnectionStringRetriever.Object, cache, secretManager.Object, logger);

            //ACT
            var nameCollection = connectionStringProvider.GetConnectionString(tenantContext);
            //ASSERT
            tenantConnectionStringRetriever.Verify(x => x.GetTenantAttributes(It.Is<TenantHttpContext>(c => c == tenantContext), CancellationToken.None), Times.Never);
            Assert.AreEqual(expectedNameCollection.AllKeys, nameCollection.Keys);
            Assert.AreEqual(expectedNameCollection.GetValues(0), nameCollection.GetValues(0));
            Assert.AreEqual(expectedNameCollection.GetValues(1), nameCollection.GetValues(1));
            Assert.AreEqual(expectedNameCollection.GetValues(2), nameCollection.GetValues(2));
            Assert.AreEqual(expectedNameCollection.GetValues(3), nameCollection.GetValues(3));
        }
    }
}