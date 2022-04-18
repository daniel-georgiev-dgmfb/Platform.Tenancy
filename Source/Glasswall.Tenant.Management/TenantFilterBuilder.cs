using System;
using System.Linq;

namespace Platform.Tenant.Management
{
	public class TenantFilterBuilder : ITenantFilterBuilder
    {
        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Guid tenantId) where T : BaseTenantModel
        {
            if (query == null)
                throw new ArgumentNullException("query");
            return QueryableExtensions.ApplyFilter(query, tenantId);
        }

        public T AssignTenantId<T>(T item, Guid tenantId) where T : BaseTenantModel
        {
            if (item == null)
                throw new ArgumentNullException("item");
            var propertyDelegate = TenantModelExtensions.AssignTenantId(item, tenantId);
            return item;
        }
    }
}