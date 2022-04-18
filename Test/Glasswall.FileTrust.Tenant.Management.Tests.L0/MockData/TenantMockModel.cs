using System;
using Glasswall.Kernel.Data.Tenancy;

namespace Glasswall.FileTrust.Tenant.Management.Tests.L0.MockData
{
    public class TenantMockModel : BaseTenantModel
    {
        public TenantMockModel(Guid tenantId)
        {
            base.TenantId = tenantId;
        }
    }
}