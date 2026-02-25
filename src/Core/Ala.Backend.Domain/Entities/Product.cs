using Ala.Backend.Domain.Common;
using Ala.Backend.Domain.Events.Products;

namespace Ala.Backend.Domain.Entities
{
    public class Product : SoftDeleteEntity<int>
    {
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }

        private Product() { } 

        public Product(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            Price = newPrice;

            AddDomainEvent(new ProductPriceChangedEvent(Id, newPrice));
        }

        public void Delete(string deletedBy)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Product already deleted.");

            MarkAsDeleted(deletedBy);

            AddDomainEvent(new ProductSoftDeletedEvent(Id, Name));
        }
    }
}