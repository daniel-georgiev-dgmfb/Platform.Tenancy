using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Tenancy;

namespace Glasswall.Tenant.Management
{
    public class TenantIdentityProvider : ITenantIdentityProvider
    {
        private readonly IEnumerable<ITenantResolver> _resolvers;
        private readonly IConnectionStringProvider<NameValueCollection> _connectionStringProvider;
        private readonly IGWLogger<TenantIdentityProvider> _logger;

        public TenantIdentityProvider(IConnectionStringProvider<NameValueCollection> connectionStringProvider, IGWLogger<TenantIdentityProvider> logger) : this(connectionStringProvider, Enumerable.Empty<ITenantResolver>(), logger)
        {
        }

        public TenantIdentityProvider(IConnectionStringProvider<NameValueCollection> connectionStringProvider, IEnumerable<ITenantResolver> resolvers, IGWLogger<TenantIdentityProvider> logger)
        {
            if (resolvers == null)
                throw new ArgumentNullException(nameof(resolvers));
            if (connectionStringProvider == null)
                throw new ArgumentNullException(nameof(connectionStringProvider));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._resolvers = resolvers;
            this._connectionStringProvider = connectionStringProvider;
            this._logger = logger;
        }

        public async Task<TenantDescriptor> GetTenantDescriptor(TenantResolutionContext context, IEnumerable<ITenantResolver> resolvers)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (resolvers == null)
                throw new ArgumentNullException(nameof(resolvers));

            this._logger.Log(LogLevel.Information, 0, String.Format("Resolving tenant identifier. Register resolvers: {0}", this._resolvers.Aggregate(new StringBuilder(), (b, next) => { b.AppendFormat("Resolver type: {0}\r\n", next.GetType().FullName); return b; })), (Exception)null, (s, e) => s.ToString());
            var seed = new Func<TenantResolutionContext, Task>(c => Task.CompletedTask);
            var del = resolvers.Aggregate(seed, (x, next) => new Func<TenantResolutionContext, Task>(c => next.ResolveTenant(c, x)));
            var task = await del(context)
                .ContinueWith(async t =>
                {
                    if (t.IsFaulted)
                        throw t.Exception;
                    if (!context.IsResolved)
                        return;
                    var httpcontext = new TenantHttpContext(context.TenantDescriptor, context.TenantEndpoint, context.ClientCredentials);
                    var connectionString = await this.GetTenantAttributes(httpcontext);
                    context.TenantDescriptor.TenantAttributes.Add(connectionString);
                });
            await task;
            this._logger.Log(LogLevel.Information, 0, String.Format("Tenant identifier has {0} been resolved. Tenant identifier: {1}", context.IsResolved ? String.Empty : "not", context.IsResolved ? context.TenantDescriptor.ToString() : "null"), (Exception)null, (s, e) => s.ToString());
            if (context.IsResolved)
                return context.TenantDescriptor;
            return null;   
        }

        public Task<TenantDescriptor> GetTenantDescriptor(TenantResolutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return this.GetTenantDescriptor(context, this._resolvers);
        }
        
        private Task<NameValueCollection> GetTenantAttributes(TenantHttpContext context)
        {
            this._logger.Log(LogLevel.Debug, 0, String.Format("Retrieveing tenant attributes."), (Exception)null, (s, e) => s.ToString());
            return Task.Factory.StartNew<NameValueCollection>(() => this._connectionStringProvider.GetConnectionString(context));
        }
    }
}