using Ala.Backend.Domain.Abstractions;

namespace Ala.Backend.Domain.Events.Products
{
    public sealed class ProductPriceChangedEvent : IDomainEvent
    {
        public int ProductId { get; }
        public decimal NewPrice { get; }
        public DateTime OccurredOn { get; }

        public ProductPriceChangedEvent(int productId, decimal newPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
            OccurredOn = DateTime.UtcNow;
        }
    }
}