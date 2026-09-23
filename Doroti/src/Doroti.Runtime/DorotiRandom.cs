namespace Doroti.Runtime;

/// <summary>Fold a 64-bit framework seed into the seed accepted by System.Random.</summary>
public static class DorotiRandom
{
    public static Random FromSeed(long seed) =>
        new(unchecked((int)(seed ^ (seed >> 32))));
}
