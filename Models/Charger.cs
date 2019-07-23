namespace EnergyApp.Data
{
    public enum Type
    {
        Normal, Additive
    }

    public class Charger
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float MaxCurrent { get; set; }
        public int OutletID { get; set; }
    }
}