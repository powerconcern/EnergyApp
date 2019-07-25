using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CustomerNumber { get; set; }
        public int MeterID { get; set; }
        public int ChargerID { get; set; }
        public virtual ICollection<Charger> Chargers { get; set; }
        public virtual ICollection<Meter> Meters { get; set; }
    }
}