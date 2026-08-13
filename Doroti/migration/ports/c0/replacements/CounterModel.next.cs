namespace Doroti.Generated.C0;

public partial class CounterModel
{
    public int next(int input)
    {
        var step = input < 1 ? 1 : input;
        return seed + Math.Min(step, 10);
    }
}
