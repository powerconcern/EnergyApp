using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class CMPAssignment
    {
        public int MeterID { get; set; }
        public int ChargerID { get; set; }
        public int PartnerID { get; set; }
        public Meter Meter { get; set; }
        public Charger Charger { get; set; }
        public Partner Partner { get; set; }
        public ICollection<Adjustment> Adjustments { get; set; }
    }
}