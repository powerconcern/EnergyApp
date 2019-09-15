using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Partner
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserReference { get; set; }
        public ICollection<CMPAssignment> CMPAssignments { get; set; }
        public PartnerType? Type { get; set; }
    }
    public enum PartnerType
    {
        None,
        Kund,
        Förening,
        Företag
    }
}