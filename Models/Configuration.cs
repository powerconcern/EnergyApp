using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerConcern.Models
{
    public class Configuration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ConfigurationID { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
    }
}