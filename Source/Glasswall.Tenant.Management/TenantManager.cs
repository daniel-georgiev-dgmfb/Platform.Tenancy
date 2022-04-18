using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Glasswall.Tenant.Management.Tests.L1")]
[assembly: InternalsVisibleTo("Glasswall.FileTrust.Tenant.Management.Tests.L0")]
namespace Platform.Tenant.Management
{
	public class TenantManager : ITenantManager
    {
        private readonly ITenantIdentityProvider _tenantIdentityProvider;
        private readonly IGWLogger<TenantManager> _logger;

        public TenantManager(ITenantIdentityProvider tenantIdentityProvider, IGWLogger<TenantManager> logger)
        {
            if (tenantIdentityProvider == null)
                throw new ArgumentNullException(nameof(tenantIdentityProvider));
            
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            this._tenantIdentityProvider = tenantIdentityProvider;
            this._logger = logger;
        }
        
        public async Task<TenantDescriptor> GetTenantDescriptor(TenantResolutionContext context)
        {
            var descriptor = (TenantDescriptor)null;
            try
            {
                descriptor = await this._tenantIdentityProvider.GetTenantDescriptor(context);
            }
            catch(Exception ex)
            {
                this._logger.Log(LogLevel.Error, 0, String.Empty, ex, (s, e) => e.ToString());
            }

            return descriptor;
        }
    }
}