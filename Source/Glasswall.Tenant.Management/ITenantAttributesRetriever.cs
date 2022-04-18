using System.Threading;
using System.Threading.Tasks;
using Glasswall.Tenant.Management.Models;

namespace Glasswall.Tenant.Management
{
    public interface ITenantAttributesRetriever
    {
        Task<TenantModel> GetTenantAttributes(TenantHttpContext tenantHttpContext, CancellationToken cancellationToken);
    }
}