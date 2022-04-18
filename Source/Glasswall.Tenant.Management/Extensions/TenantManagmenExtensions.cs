using System;
using System.Collections.Specialized;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.Data.Tenancy;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Tenancy;
using Glasswall.Platform.Web.Api.Client.Extensions;

namespace Glasswall.Tenant.Management.Extensions
{
    public static class TenantManagmenExtensions
    {
         public static IDependencyResolver AddTenancy(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            if(!dependencyResolver.Contains<ITenantManager, TenantManager>())
                dependencyResolver.RegisterType<ITenantManager, TenantManager>(Lifetime.Transient);

            if (!dependencyResolver.Contains<IConnectionStringProvider<NameValueCollection>, TenantAttributesProvider>())
               dependencyResolver.RegisterType<IConnectionStringProvider<NameValueCollection>, TenantAttributesProvider>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITenantAttributesParser, TenantAttributesParser>())
                dependencyResolver.RegisterType<ITenantAttributesParser, TenantAttributesParser>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITenantAttributesRetriever, TenantAttributesRetriever>())
                dependencyResolver.RegisterType<ITenantAttributesRetriever, TenantAttributesRetriever>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITenantIdentityProvider, TenantIdentityProvider>())
                dependencyResolver.RegisterType<ITenantIdentityProvider, TenantIdentityProvider>(Lifetime.Transient);

            HttpClientExtensions.AddApiClient(dependencyResolver);
            return dependencyResolver;
        }

        public static IDependencyResolver AddTenanFiltering(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            if (!dependencyResolver.Contains<ITenantFilterBuilder, TenantFilterBuilder>())
                dependencyResolver.RegisterType<ITenantFilterBuilder, TenantFilterBuilder>(Lifetime.Transient);
            
            return dependencyResolver;
        }
    }
}