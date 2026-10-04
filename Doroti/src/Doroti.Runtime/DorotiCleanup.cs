namespace Doroti.Runtime;

/// <summary>Attempts every owned cleanup, preserving all failures for the caller.</summary>
public static class DorotiCleanup
{
    public static void Run(params Action[] actions)
    {
        List<Exception>? failures = null;
        foreach (var action in actions)
        {
            try { action(); }
            catch (Exception error) { (failures ??= []).Add(error); }
        }
        if (failures is not null) throw new AggregateException("Owner cleanup failed.", failures);
    }
}
