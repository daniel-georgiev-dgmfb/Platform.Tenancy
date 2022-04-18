using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Glasswall.Tenant.Management.Tests.L0")]
namespace Platform.Tenant.Management
{
	internal class TenantAttributesProvider : IConnectionStringProvider<NameValueCollection>
    {
        private readonly ITenantAttributesRetriever _tenantConnectionStringRetriever;
        private readonly IGWLogger<TenantAttributesProvider> _logger;
        private readonly ISecretManager _secretManager;
        private readonly ICacheProvider _cache;

        public TenantAttributesProvider(ITenantAttributesRetriever tenantConnectionStringRetriever, ICacheProvider cache, ISecretManager secretManager, IGWLogger<TenantAttributesProvider> logger)
        {
            if (tenantConnectionStringRetriever == null)
                throw new ArgumentNullException(nameof(tenantConnectionStringRetriever));
           
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (secretManager == null)
                throw new ArgumentNullException(nameof(secretManager));
           
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._tenantConnectionStringRetriever = tenantConnectionStringRetriever;
            this._secretManager = secretManager;
            this._cache = cache;
            this._logger = logger;
        }

        public NameValueCollection GetConnectionString()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection GetConnectionString<TContext>(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var tenantHttpContext = context as TenantHttpContext;
            if (tenantHttpContext == null)
                throw new InvalidOperationException(String.Format("Expected context of type: {0} but it was: {1}", typeof(TenantHttpContext).FullName, context.GetType().FullName));

            this._logger.Log(LogLevel.Information, 0, String.Format("Accessing cache for entry for tenant context: {0}", tenantHttpContext), null, (s, e) => s.ToString());
            var nameCollection = this._cache.GetOrAddAsync(tenantHttpContext.TenantDescriptor.TenantId.ToString(), _ => this.GetTenantAttributesInternal(tenantHttpContext, CancellationToken.None), CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            return nameCollection;
        }

        private async Task<NameValueCollection> GetTenantAttributesInternal(TenantHttpContext tenantHttpContext, CancellationToken cancellationToken)
        {
            try
            {
                this._logger.Log(LogLevel.Information, 0, String.Format("Begin http call to tenant service."), null, (s, e) => s.ToString());
                var tenantModel = await this._tenantConnectionStringRetriever.GetTenantAttributes(tenantHttpContext, cancellationToken);
                if (tenantModel == null)
                    return null;
                this._logger.Log(LogLevel.Information, 0, String.Format("Begin http call to secret vault to resolve database password."), null, (s, e) => s.ToString());
                this._logger.Log(LogLevel.Debug, 0, String.Format("Secret name: {0}", tenantModel.ConnectionStringAttributes.SecretName), null, (s, e) => s.ToString());
                var nameValueCollection = this.ValidateRequiredAttributes(tenantModel);
                await this.AssignSecurityContext(nameValueCollection, tenantModel);
                return nameValueCollection;
            }
            catch (Exception e)
            {
                this._logger.Log(LogLevel.Error, 0, String.Empty, e, (s, ex) => ex.ToString());
                throw;
            }
        }

        private NameValueCollection ValidateRequiredAttributes(TenantModel tenantModel)
        {
            if (tenantModel == null)
                throw new ArgumentNullException(nameof(tenantModel));

            if (String.IsNullOrWhiteSpace(tenantModel.TenantName))
                throw new ArgumentNullException(nameof(tenantModel.TenantName));

            if (tenantModel.ConnectionStringAttributes == null)
                throw new ArgumentNullException(nameof(tenantModel.ConnectionStringAttributes));

            if (String.IsNullOrWhiteSpace(tenantModel.ConnectionStringAttributes.Server))
                throw new InvalidOperationException(String.Format("Required attribute: {0} is not present.", nameof(tenantModel.ConnectionStringAttributes.Server)));

            if (String.IsNullOrWhiteSpace(tenantModel.ConnectionStringAttributes.Database))
                throw new InvalidOperationException(String.Format("Required attribute: {0} is not present.", nameof(tenantModel.ConnectionStringAttributes.Database)));

            var nameValueCollection = new NameValueCollection
            {
                { "TenantName", tenantModel.TenantName },
                { "DataSource", tenantModel.ConnectionStringAttributes.Server },
                { "DataBase", tenantModel.ConnectionStringAttributes.Database}
            };
            
            return nameValueCollection;
        }

        private async Task AssignSecurityContext(NameValueCollection nameValueCollection, TenantModel connectionStringDescriptor)
        {
            if(!connectionStringDescriptor.ConnectionStringAttributes.AllowedIntergratedAutheication)
            {
                if (String.IsNullOrWhiteSpace(connectionStringDescriptor.ConnectionStringAttributes.User))
                    throw new InvalidOperationException(String.Format("Required attribute: {0} is not present.", nameof(connectionStringDescriptor.ConnectionStringAttributes.User)));
                if (String.IsNullOrWhiteSpace(connectionStringDescriptor.ConnectionStringAttributes.SecretName))
                    throw new InvalidOperationException(String.Format("Required attribute: {0} is not present.", nameof(connectionStringDescriptor.ConnectionStringAttributes.SecretName)));
            }

            var password = await this._secretManager.GetSecret(new SecretContext(connectionStringDescriptor.ConnectionStringAttributes.SecretName));
            nameValueCollection.Add("UserName", connectionStringDescriptor.ConnectionStringAttributes.User);
            nameValueCollection.Add("Password", password);
        }
    }
}