using Glasswall.Kernel.Web;
using Glasswall.Tenant.Management.Models;

namespace Glasswall.Tenant.Management
{
    public interface ITenantAttributesParser : IHttpParser<TenantModel>
    {
    }
}