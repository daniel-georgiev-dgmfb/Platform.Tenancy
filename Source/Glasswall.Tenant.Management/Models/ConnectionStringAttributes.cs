using System;

namespace Platform.Tenant.Management.Models
{
	public class ConnectionStringAttributes
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string SecretName { get; set; }
        public bool AllowedIntergratedAutheication
        {
            get
            {
                return String.IsNullOrWhiteSpace(this.User) && String.IsNullOrWhiteSpace(this.SecretName);
            }
        }
    }
}