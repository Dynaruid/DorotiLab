namespace Doroti.Skia.Rendering;

public sealed record TextureBudgetSnapshot(ulong AllocatedBytes, ulong BudgetBytes,
    ulong PeakBudgetBytes, ulong LimitBytes, ulong RequiredBytes, string Source, long RejectedAllocations);

/// <summary>Demand-sized texture allowance, bounded by a heap share and current driver headroom.
/// Reducing the allowance never releases allocations: only their owner can prove GPU retirement.</summary>
public sealed class DynamicTextureBudget
{
    private const ulong Quantum = 64 * 1024;
    private ulong _allocated, _budget, _peak, _limit, _required;
    private string _source = "unqueried";
    private long _rejected;
    public TextureBudgetSnapshot Snapshot => new(_allocated, _budget, _peak, _limit, _required, _source, _rejected);

    /// <summary>Driver usage includes this allocator and other allocations in the process.
    /// With no driver query, heap/host capacity is an estimate, not measured free memory.</summary>
    public bool TryReserve(ulong allocatedBytes, ulong additionalBytes, ulong heapBytes,
        ulong? driverBudgetBytes = null, ulong? driverUsageBytes = null, ulong? hostCapacityBytes = null)
    {
        _allocated = allocatedBytes;
        _limit = heapBytes - heapBytes / 10;
        if (hostCapacityBytes is { } host) _limit = Math.Min(_limit, host - host / 10);
        _source = "heap-size-estimate";
        if (driverBudgetBytes is > 0 && driverUsageBytes is { } usage)
        {
            var capacity = Math.Min(heapBytes, driverBudgetBytes.Value);
            var accountedUsage = Math.Max(usage, allocatedBytes);
            var free = capacity > accountedUsage ? capacity - accountedUsage : 0;
            // Keep a fifth of reported free space for Qt, Graphite and other users.
            _limit = Math.Min(_limit, Add(allocatedBytes, free - free / 5));
            _source = "VK_EXT_memory_budget";
        }
        var overflow = additionalBytes > ulong.MaxValue - allocatedBytes;
        _required = Add(allocatedBytes, additionalBytes);
        var allowed = !overflow && _required <= _limit;
        ResizeAllowance(allowed ? _required : allocatedBytes);
        if (!allowed) _rejected++;
        return allowed;
    }

    /// <summary>Call after actual allocation/free. Live and GPU-owned bytes remain an absolute floor.</summary>
    public void ObserveAllocated(ulong allocatedBytes)
    {
        _allocated = allocatedBytes;
        ResizeAllowance(allocatedBytes);
    }

    private void ResizeAllowance(ulong demand)
    {
        var target = Add(demand, demand / 4);
        target = target > ulong.MaxValue - (Quantum - 1) ? ulong.MaxValue
            : (target + Quantum - 1) / Quantum * Quantum;
        _budget = Math.Max(_allocated, Math.Min(_limit, target));
        _peak = Math.Max(_peak, _budget);
    }

    private static ulong Add(ulong left, ulong right) =>
        right > ulong.MaxValue - left ? ulong.MaxValue : left + right;
}
