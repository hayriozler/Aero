namespace Aero.Shared.Domain;

public interface IKey<T>
{
    T Id { get; }
}
