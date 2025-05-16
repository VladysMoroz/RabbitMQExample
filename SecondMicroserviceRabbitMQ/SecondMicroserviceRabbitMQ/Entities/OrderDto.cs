namespace SecondMicroserviceRabbitMQ.Entities
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Product { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
