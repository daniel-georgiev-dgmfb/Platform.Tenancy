using NUnit.Framework;
using System;
using System.Linq;

namespace Platform.FileTrust.Tenant.Management.Tests.L0
{
	[TestFixture]
    [Category("TenantFilterBuilder")]
    public class TenantFilterBuilderTests
    {
        [Test]
        public void ApplyFilter_to_queryable()
        {
            //ARRANGE
            var tenantId = Guid.NewGuid();
            var source = new[] { new TenantMockModel(tenantId), new TenantMockModel(tenantId), new TenantMockModel(Guid.NewGuid()) };
            var tenantFilterBuilder = new TenantFilterBuilder();
            //ACT
            var result = tenantFilterBuilder.ApplyFilter(source.AsQueryable(), tenantId);
            //ASSERT
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void ApplyFilter_to_queryable_null_source()
        {
            //ARRANGE
            var tenantFilterBuilder = new TenantFilterBuilder();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => tenantFilterBuilder.ApplyFilter((IQueryable<BaseTenantModel>)null, Guid.Empty));
        }

        [Test]
        public void Assign_tenant_id_to_model()
        {
            //ARRANGE
            var tenantId = Guid.NewGuid();
            var source = new TenantMockModel(Guid.Empty);
            var tenantFilterBuilder = new TenantFilterBuilder();
            //ACT
            var result = tenantFilterBuilder.AssignTenantId(source, tenantId);
            //ASSERT
            Assert.AreEqual(tenantId, result.TenantId);
            Assert.AreEqual(tenantId, source.TenantId);
        }

        [Test]
        public void Assign_tenant_id_to_model_null_source()
        {
            //ARRANGE
            var tenantFilterBuilder = new TenantFilterBuilder();
            //ACT

            //ASSERT
            Assert.Throws<ArgumentNullException>(() => tenantFilterBuilder.AssignTenantId((BaseTenantModel)null, Guid.Empty));
        }
    }
}