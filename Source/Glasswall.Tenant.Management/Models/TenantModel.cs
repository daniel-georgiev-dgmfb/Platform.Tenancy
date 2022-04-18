using System;

namespace Glasswall.Tenant.Management.Models
{
    public class TenantModel
    {
        public string TenantName { get; set; }
        public string DisplayName { get; set; }
        public ConnectionStringAttributes ConnectionStringAttributes { get; set; }
    }
}