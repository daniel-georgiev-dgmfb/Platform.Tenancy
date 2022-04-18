using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Kernel.Tenancy;
using Glasswall.Kernel.Web;
using Glasswall.Kernel.Web.Authorisation;
using Glasswall.Platform.Web.Api.Client;
using Glasswall.Tenant.Management.Models;
using Glasswall.Tenant.Management.Tests.L0.MockData;
using Moq;
using NUnit.Framework;

namespace Glasswall.Tenant.Management.Tests.L0
{
    [TestFixture]
    [Category("TenantConnectionStringRetriever")]
    internal class TenantConnectionStringRetrieverTests
    {
        [Test]
        [Category("TenantConnectionStringRetriever")]
        public void When_instantiate_with_resource_retriever_null()
        {
            //ARRANGE
            var resourceRetriever = new Mock<IApiClient>();
            var parser = new Mock<ITenantAttributesParser>();
            var logger = new MockLogger<TenantAttributesRetriever>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesRetriever(null, parser.Object, logger));
        }
        
        [Test]
        [Category("TenantConnectionStringRetriever")]
        public void When_instantiate_with_parser_null()
        {
            //ARRANGE
            var resourceRetriever = new Mock<IApiClient>();
            var parser = new Mock<ITenantAttributesParser>();
            var logger = new MockLogger<TenantAttributesRetriever>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesRetriever(resourceRetriever.Object, null, logger));
        }

        [Test]
        [Category("TenantConnectionStringRetriever")]
        public void When_instantiate_with_logger_null()
        {
            //ARRANGE
            var resourceRetriever = new Mock<IApiClient>();
            var parser = new Mock<ITenantAttributesParser>();
            var logger = new MockLogger<TenantAttributesRetriever>();
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesRetriever(resourceRetriever.Object, parser.Object, null));
        }

        [Test]
        [Category("TenantConnectionStringRetriever")]
        public async Task When_call_get_connection_string_send_async_called_()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent("Response context");
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var bearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, bearerTokenContext.Object);
           
            var resourceRetriever = new Mock<IApiClient>();
            resourceRetriever.Setup(x => x.SendAsync(It.IsAny<RequestContext>(), HttpMethod.Get, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(response);
            
            var parser = new Mock<ITenantAttributesParser>();
            var logger = new MockLogger<TenantAttributesRetriever>();
            var retriever = new TenantAttributesRetriever(resourceRetriever.Object, parser.Object, logger);
            //ACT
            await retriever.GetTenantAttributes(tenantContext, CancellationToken.None);
            //ASSERT
            resourceRetriever.Verify(x => x.SendAsync(It.IsAny<RequestContext>(), HttpMethod.Get, It.IsAny<CancellationToken>(), false));
        }

        [Test]
        [Category("TenantConnectionStringRetriever")]
        public async Task When_call_get_connection_string_get_with_200_response_parse_connection_string_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent("Response context");
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var mockBearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, mockBearerTokenContext.Object);
            var tokenManager = new Mock<IBearerTokenManager>();
            tokenManager.Setup(x => x.GetToken(It.Is<IBearerTokenContext>(_ => _ == mockBearerTokenContext.Object), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TokenDescriptor("bearer", "token", DateTimeOffset.Now, 10))
                .Verifiable();
            var resourceRetriever = new Mock<IApiClient>();
            resourceRetriever.Setup(x => x.SendAsync(It.IsAny<RequestContext>(), HttpMethod.Get, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(response);

            var parser = new Mock<ITenantAttributesParser>();
            parser.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new TenantModel());
            var logger = new MockLogger<TenantAttributesRetriever>();
            var retriever = new TenantAttributesRetriever(resourceRetriever.Object, parser.Object, logger);
            //ACT
            await retriever.GetTenantAttributes(tenantContext, CancellationToken.None);
            //ASSERT

            parser.Verify(x => x.Parse(It.IsAny<string>(), It.IsAny<bool>()));
        }

        [Test]
        [Category("TenantConnectionStringRetriever")]
        public async Task When_call_get_connection_string_get_with_400_response_parse_connection_string_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            response.Content = new StringContent("Response 400(Not found)");
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var mockBearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, mockBearerTokenContext.Object);
            var tokenManager = new Mock<IBearerTokenManager>();
            tokenManager.Setup(x => x.GetToken(It.Is<IBearerTokenContext>(_ => _ == mockBearerTokenContext.Object), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TokenDescriptor("bearer", "token", DateTimeOffset.Now, 10))
                .Verifiable();
            var resourceRetriever = new Mock<IApiClient>();
            resourceRetriever.Setup(x => x.SendAsync(It.IsAny<RequestContext>(), HttpMethod.Get, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(response);

            var parser = new Mock<ITenantAttributesParser>();
            parser.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new TenantModel());
            var logger = new MockLogger<TenantAttributesRetriever>();
            var retriever = new TenantAttributesRetriever(resourceRetriever.Object, parser.Object, logger);
            //ACT
            await retriever.GetTenantAttributes(tenantContext, CancellationToken.None);
            //ASSERT

            parser.Verify(x => x.Parse(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        [Category("TenantConnectionStringRetriever")]
        public async Task When_call_get_connection_string_get_with_500_response_parse_connection_string_called()
        {
            //ARRANGE
            Guid tenantId = Guid.NewGuid();
            var tokenEndpointAddress = "https://cas/token/";
            var tenantEndpointAddress = "https://localhost/tenant/";
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            response.Content = new StringContent("Response 400(Not found)");
            var tenantDescriptor = new TenantDescriptor(tenantId);
            var tokenEndPont = new Endpoint(tokenEndpointAddress);
            var tenantEndpoint = new Endpoint(tenantEndpointAddress);
            var mockBearerTokenContext = new Mock<IBearerTokenContext>();
            var tenantContext = new TenantHttpContext(tenantDescriptor, tenantEndpoint, mockBearerTokenContext.Object);
            var tokenManager = new Mock<IBearerTokenManager>();
            tokenManager.Setup(x => x.GetToken(It.Is<IBearerTokenContext>(_ => _ == mockBearerTokenContext.Object), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TokenDescriptor("bearer", "token", DateTimeOffset.Now, 10))
                .Verifiable();
            var resourceRetriever = new Mock<IApiClient>();
            resourceRetriever.Setup(x => x.SendAsync(It.IsAny<RequestContext>(), HttpMethod.Get, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(response);

            var parser = new Mock<ITenantAttributesParser>();
            parser.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new TenantModel());
            var logger = new MockLogger<TenantAttributesRetriever>();
            var retriever = new TenantAttributesRetriever(resourceRetriever.Object, parser.Object, logger);
            //ACT

            //ASSERT
            Assert.ThrowsAsync<HttpRequestException>(() => retriever.GetTenantAttributes(tenantContext, CancellationToken.None));
        }
    }
}
