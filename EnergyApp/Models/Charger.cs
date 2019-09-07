using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Charger
    {
        public int ID { get; set; }
        public int OutletID { get; set; }
        public string Name { get; set; }
        public float MaxCurrent { get; set; }
        public virtual ICollection<Outlet> Outlets { get; set; }
    }
}