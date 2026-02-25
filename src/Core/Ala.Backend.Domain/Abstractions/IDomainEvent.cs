namespace Ala.Backend.Domain.Abstractions
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}