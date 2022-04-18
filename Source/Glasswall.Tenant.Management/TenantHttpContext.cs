using System;
using Glasswall.Kernel.Tenancy;
using Glasswall.Kernel.Web;
using Glasswall.Kernel.Web.Authorisation;

namespace Glasswall.Tenant.Management
{
    public class TenantHttpContext
    {
        public TenantHttpContext(TenantDescriptor tenantDescriptor, Endpoint tenantEndpoint, IBearerTokenContext clientCredentials)
        {
            if (tenantDescriptor == null)
                throw new ArgumentNullException(nameof(tenantDescriptor));
            
            if (tenantEndpoint == null)
                throw new ArgumentNullException(nameof(tenantEndpoint));
            if (clientCredentials == null)
                throw new ArgumentNullException(nameof(clientCredentials));
            this.TenantDescriptor = tenantDescriptor;
            this.ClientCredentials = clientCredentials;
            this.TenantEndpoint = tenantEndpoint;
        }

        public TenantDescriptor TenantDescriptor { get; }

        public Endpoint TenantEndpoint { get; }

        public IBearerTokenContext ClientCredentials { get; }

        public override string ToString()
        {
            return String.Format("Tenant: {0}, endpoint: {1}", this.TenantDescriptor.ToString(), this.TenantEndpoint.ToString());
        }
    }
}