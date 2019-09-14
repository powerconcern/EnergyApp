using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Charger
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float MaxCurrent { get; set; }
        public ICollection<Outlet> Outlets { get; set; }
    }
}