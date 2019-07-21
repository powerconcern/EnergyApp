namespace PowerConcern.Models
{
    public enum Type
    {
        Normal, Positive
    }

    public class Meter
    {
        public int ID { get; set; }
        public int Name { get; set; }
        public int MaxCurrent { get; set; }
        public Type? Type { get; set; }
        public int ChargerID { get; set; }
    }
}