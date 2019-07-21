using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyApp.Data
{
    public class Configuration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ConfigurationID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}