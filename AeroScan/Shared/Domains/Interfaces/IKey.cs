namespace Domain.Interfaces;

public interface IKey<T>
{
    T Id { get; }
}
