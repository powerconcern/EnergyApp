using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CustomerNumber { get; set; }
        public CustomerType? Type { get; set; }
        public ICollection<CMCAssign> CMCAssigns { get; set; }
    }
    public enum CustomerType
    {
        None,
        Kund,
        Förening,
        Företag
    }
}