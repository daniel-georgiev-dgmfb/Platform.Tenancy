using Moq;
using NUnit.Framework;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.Tenant.Management.Tests.L1
{
	[TestFixture]
    [Category("TenantConnectionStringRetriever")]
    [Ignore("Require tenant endpoint")]
    public class TenantConnectionStringRetrieverTests
    {
        [Test]
        public async Task When_tenant_exists()
        {
            //ARRANGE
            var tenantId = Guid.Parse("9629FA3E-0E12-4942-9288-127E9A0B6B31");
            var backchannelCertificateValidator = new Mock<IBackchannelCertificateValidator>();
            backchannelCertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate2>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var logger = new MockLogger<TenantAttributesRetriever>();
            var httpClient = new HttpClient(backchannelCertificateValidator.Object, new MockLogger<HttpClient>());
            var cache = new MemoryCacheRuntimeImplementor();
            var settings = new DefaultSettingsProvider();
            var serialiser = new NSJsonSerializer(settings);
            var tokenParser = new BearerTokenParser(serialiser);
            var bearerTokenManager = new TokenManager(httpClient, cache, tokenParser, new MockLogger<TokenManager>());
            var apiClient = new ApiClient(bearerTokenManager, httpClient, new MockLogger<ApiClient>());
            var connectionStringParser = new TenantAttributesParser(serialiser);
            var tenantConnectionStringRetriever = new TenantAttributesRetriever(apiClient, connectionStringParser, logger);
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var endpoint = new Endpoint("https://localhost:5001/api/tenant");
            var credentials = new ClientSecretTokenContext("service", "Glasswall", new Endpoint("https://cas.wotsits.filetrust.io/Connect/Token"));
            var context = new TenantHttpContext(tenantDescriptor, endpoint, credentials);
            //ACT
            var connectionString = await tenantConnectionStringRetriever.GetTenantAttributes(context, CancellationToken.None);
            //ASSERT
            Assert.IsNotNull(connectionString);
            Assert.IsNotNull(connectionString.ConnectionStringAttributes.Database);
            Assert.IsNotNull(connectionString.ConnectionStringAttributes.Server);
            Assert.IsNotNull(connectionString.ConnectionStringAttributes.User);
            Assert.IsNotNull(connectionString.ConnectionStringAttributes.SecretName);
        }

        [Test]
        public async Task When_tenant_does_not_exist()
        {
            //ARRANGE
            var tenantId = Guid.NewGuid();
            var backchannelCertificateValidator = new Mock<IBackchannelCertificateValidator>();
            backchannelCertificateValidator.Setup(x => x.Validate(It.IsAny<object>(), It.IsAny<X509Certificate2>(), It.IsAny<X509Chain>(), It.IsAny<SslPolicyErrors>()))
                .Returns(true);
            var logger = new MockLogger<TenantAttributesRetriever>();
            var httpClient = new HttpClient(backchannelCertificateValidator.Object, new MockLogger<HttpClient>());
            var cache = new MemoryCacheRuntimeImplementor();
            var settings = new DefaultSettingsProvider();
            var serialiser = new NSJsonSerializer(settings);
            var tokenParser = new BearerTokenParser(serialiser);
            var bearerTokenManager = new TokenManager(httpClient, cache, tokenParser, new MockLogger<TokenManager>());
            var connectionStringParser = new TenantAttributesParser(serialiser);
            var apiClient = new ApiClient(bearerTokenManager, httpClient, new MockLogger<ApiClient>());
            var tenantConnectionStringRetriever = new TenantAttributesRetriever(apiClient, connectionStringParser, logger);
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var endpoint = new Endpoint("https://localhost:5001/api/tenant");
            var credentials = new ClientSecretTokenContext("service", "Glasswall", new Endpoint("https://cas.wotsits.filetrust.io/Connect/Token"));
            var context = new TenantHttpContext(tenantDescriptor, endpoint, credentials);
            
            //ACT
            var connectionString = await tenantConnectionStringRetriever.GetTenantAttributes(context, CancellationToken.None);
            
            //ASSERT
            Assert.IsNull(connectionString);
        }
    }
}