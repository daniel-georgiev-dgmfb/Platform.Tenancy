using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.Tenant.Management
{
	internal class TenantAttributesRetriever : ITenantAttributesRetriever
    {
        private readonly IApiClient _apiClient;
        private readonly ITenantAttributesParser _connectionStringParser;
        private readonly IGWLogger<TenantAttributesRetriever> _logger;
        
        public TenantAttributesRetriever(IApiClient apiClient, ITenantAttributesParser connectionStringParser, IGWLogger<TenantAttributesRetriever> logger)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));
           
            if (connectionStringParser == null)
                throw new ArgumentNullException(nameof(connectionStringParser));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            this._connectionStringParser = connectionStringParser;
            
            this._apiClient = apiClient;
            this._logger = logger;
        }
        public async Task<TenantModel> GetTenantAttributes(TenantHttpContext tenantHttpContext, CancellationToken cancellationToken)
        {
            try
            {
                var route = String.Format("{0}/{1}", tenantHttpContext.TenantEndpoint.Endpont.AbsoluteUri, tenantHttpContext.TenantDescriptor.TenantId);
                var endpoint = new Endpoint(route);
                var request = new RequestContext(endpoint, tenantHttpContext.ClientCredentials);
                this._logger.Log(LogLevel.Information, 0, String.Format("Calling tenant endpoint: {0}", route), (Exception)null, (s, e) => s.ToString());
                var tenantSummaryResponse = await this._apiClient.SendAsync(request, HttpMethod.Get, cancellationToken, false);
                var tenantSummary = await this.GetTenantSummary(tenantSummaryResponse);
                if (tenantSummary == null)
                    return null;

                var tenantModel = await this._connectionStringParser.Parse(tenantSummary, true);
                return tenantModel;
            }
            catch (HttpRequestException e)
            {
                this._logger.Log(LogLevel.Error, 0, String.Empty, e, (s, ex) => ex.ToString());
                throw;
            }
        }

        private async Task<string> GetTenantSummary(HttpResponseMessage response)
        {
            this._logger.Log(LogLevel.Debug, 0, String.Format("Tenant endpoint raw response code: {0}", response.StatusCode), (Exception)null, (s, e) => s.ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            this._logger.Log(LogLevel.Debug, 0, String.Format("Tenant endpoint raw response: {0}", content), (Exception)null, (s, e) => s.ToString());
            return content;
        }
    }
}