using System;

namespace EnergyApp.Data
{
    public class CMCAssign
    {
        public DateTime AddedDate { get; set; }
        public int MeterID { get; set; }
        public int ChargerID { get; set; }
        public int CustomerID { get; set; }
        public Meter Meter { get; set; }
        public Charger Charger { get; set; }
        public Customer Customer { get; set; }
    }
}