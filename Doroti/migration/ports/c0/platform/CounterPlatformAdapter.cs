namespace Doroti.Generated.C0;

public interface ICounterPlatformPort
{
    int Offset { get; }
}

public sealed class CounterPlatformAdapter
{
    private readonly ICounterPlatformPort _port;

    public CounterPlatformAdapter(ICounterPlatformPort port)
    {
        _port = port ?? throw new ArgumentNullException(nameof(port));
    }

    public int Next(CounterModel model, int input) => model.next(input) + _port.Offset;
}
