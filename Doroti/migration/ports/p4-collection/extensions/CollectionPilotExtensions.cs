namespace Doroti.Collection;

public static class CollectionPilotExtensions
{
    public static string IdentityLabel<T>(this T value) => $"identity:{identityFunctions.identity(value)}";
}
