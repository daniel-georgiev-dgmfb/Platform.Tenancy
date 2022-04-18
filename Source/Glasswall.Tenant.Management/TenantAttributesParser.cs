using System;
using System.Threading.Tasks;
using Glasswall.Kernel.Serialisation;
using Glasswall.Tenant.Management.Models;

namespace Glasswall.Tenant.Management
{
    internal class TenantAttributesParser : ITenantAttributesParser
    {
        private readonly IJsonSerialiser _serialiser;
        public TenantAttributesParser(IJsonSerialiser serialiser)
        {
            if (serialiser == null)
                throw new ArgumentNullException(nameof(serialiser));

            this._serialiser = serialiser;
        }
        public async Task<TenantModel> Parse(string source, bool throwOnFailure)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));
            var model = await this._serialiser.DeserialiseFromJson<ConnectionStringModel>(source);

            if (throwOnFailure && model == null)
                throw new InvalidOperationException(String.Format("Cannot deserialise response: {0} to type: {1}", source, typeof(ConnectionStringModel).FullName));
            
            return model != null ? new TenantModel
            {
                TenantName = model.name,
                DisplayName = model.displayName,
                ConnectionStringAttributes = new ConnectionStringAttributes
                {
                    Server = model.shard,
                    Database = model.database,
                    User = model.signInUserId,
                    SecretName = model.signInSecret
                }
            } : null;
        }

        public bool TryParse(string sourse, out TenantModel result)
        {
            result = null;
            try
            {
                result = this.Parse(sourse, true).GetAwaiter().GetResult();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        internal class ConnectionStringModel
        {
            public string name { get; set; }
            public string displayName { get; set; }
            public string shard { get; set; }
            public string database { get; set; }
            public string signInUserId { get; set; }
            public string signInSecret { get; set; }
        }
    }
}