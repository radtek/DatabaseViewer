using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Londeck.Net.Data
{
    public class ReadWebConfigValues : System.Configuration.ConfigurationSection
    {
        public ReadWebConfigValues()
        { }

        [ConfigurationProperty("Logon", IsRequired = false)]
        public string Logon
        {
            get
            {
                return (string)this["Logon"];
            }
        }

        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
        }
        [ConfigurationProperty("Name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
        }
    }
}
