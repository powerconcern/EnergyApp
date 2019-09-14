namespace EnergyApp.Data
{
    public enum OutletType
    {
        Type2, Schuko
    }

    public class Outlet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ChargerID { get; set; }
        public float MaxCurrent { get; set; }
        public OutletType? Type { get; set; }
        public Charger Charger { get; set; }
    }
}