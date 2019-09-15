using System;
using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class ChargeSession
    {
        public int ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Kwh { get; set; }
        public int OutletID { get; set; }
        public int ChargerID { get; set; }
        public Charger Charger { get; set; }
    }
}