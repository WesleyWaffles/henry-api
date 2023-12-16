namespace Henry.Api.Data
{
    public class ProviderAvailability
    {
        public int Id { get; set; }
        public required Provider Provider { get; set; }
        public DateOnly AvailableOn { get; set; }
        public TimeOnly AvailableFrom { get; set; }
        public TimeOnly AvailableTo { get; set; }
    }
}
