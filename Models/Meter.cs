namespace EnergyApp.Data
{
    public enum Type
    {
        Normal, Additive
    }

    public class Meter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float MaxCurrent { get; set; }
        public Type? Type { get; set; }
        public int ChargerID { get; set; }
    }
}