namespace Aero.Shared.Domain;

public abstract class Entity
{
}
public abstract class Entity<T> : Entity, IKey<T>
{
    public T Id { get; private set; } = default!;
}
