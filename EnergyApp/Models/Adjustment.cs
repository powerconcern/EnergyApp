using System;
using System.Collections.Generic;

namespace EnergyApp.Data
{
    public class Adjustment
    {
        public int ID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Current { get; set; }
        public int iPhase { get; set; }
    }
}