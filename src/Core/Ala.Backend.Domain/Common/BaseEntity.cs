using Ala.Backend.Domain.Abstractions;

namespace Ala.Backend.Domain.Common
{
    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>, IHasDomainEvents where TId : notnull
    {
        public TId Id { get; protected set; } = default!;

        public bool IsTransient =>
            EqualityComparer<TId>.Default.Equals(Id, default!);

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents =>
            _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public bool Equals(BaseEntity<TId>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (IsTransient || other.IsTransient)
                return false;

            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity<TId> other)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            if (IsTransient)
                return base.GetHashCode();

            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }

        public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
            => Equals(left, right);

        public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
            => !Equals(left, right);

        public override string ToString()
            => $"{GetType().Name} [Id={Id}]";
    }
}