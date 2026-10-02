using Doroti.Skia.Rendering;

internal static class TextureBudgetRegression
{
    internal static void Run()
    {
        const ulong mib = 1024 * 1024;
        var budget = new DynamicTextureBudget();
        // Actual byte demand from larger extents/DPR and multiple R/P banks can exceed 128 MiB.
        if (!budget.TryReserve(96 * mib, 96 * mib, 1024 * mib)
            || budget.Snapshot.BudgetBytes < 192 * mib || budget.Snapshot.BudgetBytes > 512 * mib)
            throw new Exception("Texture demand did not grow within the heap capacity.");
        budget.ObserveAllocated(192 * mib);
        if (budget.TryReserve(192 * mib, 16 * mib, 1024 * mib, 256 * mib, 255 * mib)
            || budget.Snapshot.AllocatedBytes != 192 * mib || budget.Snapshot.BudgetBytes < 192 * mib)
            throw new Exception("Memory pressure ignored headroom or released live/GPU-owned allocations.");
        budget.ObserveAllocated(16 * mib);
        if (budget.Snapshot.BudgetBytes >= 192 * mib || budget.Snapshot.BudgetBytes < 16 * mib)
            throw new Exception("Texture allowance did not shrink after proven retirement.");
        if (!budget.TryReserve(16 * mib, 8 * mib, 1024 * mib, 256 * mib, 64 * mib)
            || budget.Snapshot.Source != "VK_EXT_memory_budget")
            throw new Exception("Texture allowance did not recover when pressure eased.");
        if (budget.TryReserve(0, 231 * mib, 256 * mib)
            || budget.TryReserve(0, 116 * mib, 1024 * mib, hostCapacityBytes: 128 * mib)
            || budget.TryReserve(ulong.MaxValue - 1, 2, ulong.MaxValue))
            throw new Exception("Texture allowance exceeded device/host capacity or overflowed.");
        budget.ObserveAllocated(0);
        if (budget.Snapshot.BudgetBytes != 0 || budget.Snapshot.AllocatedBytes != 0)
            throw new Exception("Empty allocator retained a texture allowance.");
        Console.WriteLine("PASS: dynamic texture demand, driver pressure, retained GPU ownership, shrink/recovery and overflow bounds.");
    }
}
