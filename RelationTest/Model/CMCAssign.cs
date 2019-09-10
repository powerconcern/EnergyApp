using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class CMCAssign
    {
        public int ID { get; set; }
        public int MeterID { get; set; }
        public int ChargerID { get; set; }
        public int CusomerID { get; set; }
        public Meter Meter { get; set; }
        public Charger Charger { get; set; }
        public Customer Customer { get; set; }
    }
}