using System.Threading;
using System.Threading.Tasks;

namespace Platform.Tenant.Management
{
	public interface ITenantAttributesRetriever
    {
        Task<TenantModel> GetTenantAttributes(TenantHttpContext tenantHttpContext, CancellationToken cancellationToken);
    }
}