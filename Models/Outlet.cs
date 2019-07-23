namespace EnergyApp.Data
{
    public enum Type
    {
        Type2, Schuko
    }

    public class Meter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float MaxCurrent { get; set; }
        public Type? Type { get; set; }
    }
}