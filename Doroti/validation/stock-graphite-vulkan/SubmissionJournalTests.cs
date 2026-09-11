using Silk.NET.Vulkan;

internal static class SubmissionJournalTests
{
    public static object Run()
    {
        var passed = new List<string>();
        void Test(string name, Action<SubmissionJournal> action)
        { var j = new SubmissionJournal(); j.Register(1, ImageLayout.General, 0); j.Allocate(10, 100); j.Allocate(11, 100); action(j); passed.Add(name); }
        void Record(SubmissionJournal j, nint cb, ImageLayout before, ImageLayout after)
        { j.Begin(cb); j.Barrier(cb, 1, before, after, Vk.QueueFamilyIgnored, Vk.QueueFamilyIgnored); j.End(cb); }
        void Expect(SubmissionJournal j, ImageLayout state)
        { if (j.Images[1].Layout != state) throw new Exception("Wrong scheduled state: " + j.Images[1].Layout); }
        void Reject(Action action)
        { try { action(); } catch (InvalidOperationException) { return; } catch (NotSupportedException) { return; } throw new Exception("Invalid journal accepted."); }
        Test("recording-is-not-submission", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); Expect(j, ImageLayout.General); });
        Test("actual-submit-order", j => {
            Record(j, 10, ImageLayout.TransferDstOptimal, ImageLayout.TransferSrcOptimal);
            Record(j, 11, ImageLayout.General, ImageLayout.TransferDstOptimal);
            j.Submit([11, 10], Result.Success); Expect(j, ImageLayout.TransferSrcOptimal);
        });
        Test("failed-submit-does-not-commit", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.Submit([10], Result.ErrorOutOfDeviceMemory); Expect(j, ImageLayout.General); });
        Test("invalid-batch-is-atomic", j => {
            Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); Record(j, 11, ImageLayout.TransferDstOptimal, ImageLayout.General);
            Reject(() => j.Submit([10, 11], Result.Success)); Expect(j, ImageLayout.General);
        });
        Test("reset-discards-unsubmitted", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.ResetPool(100); j.End(10); j.Submit([10], Result.Success); Expect(j, ImageLayout.General); });
        Test("rerecord-replaces-journal", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); Record(j, 10, ImageLayout.General, ImageLayout.TransferDstOptimal); j.Submit([10], Result.Success); Expect(j, ImageLayout.TransferDstOptimal); });
        Test("secondary-execution", j => { Record(j, 11, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.Begin(10); j.Secondary(10, 11); j.End(10); j.Submit([10], Result.Success); Expect(j, ImageLayout.TransferSrcOptimal); });
        Test("secondary-reset-invalidates-parent", j => { Record(j, 11, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.Begin(10); j.Secondary(10, 11); j.End(10); j.Begin(11); j.End(11); Reject(() => j.Submit([10], Result.Success)); });
        Test("secondary-handle-reuse", j => { Record(j, 11, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.Begin(10); j.Secondary(10, 11); j.End(10); j.Free(11); j.Allocate(11, 100); j.Begin(11); j.End(11); Reject(() => j.Submit([10], Result.Success)); });
        Test("image-handle-reuse", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.RemoveImage(1); j.Register(1, ImageLayout.General, 0); Reject(() => j.Submit([10], Result.Success)); Expect(j, ImageLayout.General); });
        Test("foreign-owner-rejected", j => { j.Begin(10); j.Barrier(10, 1, ImageLayout.General, ImageLayout.General, 0, Vk.QueueFamilyExternal); j.End(10); Reject(() => j.Submit([10], Result.Success)); });
        Test("copy-restores-layout", j => { j.Begin(10); j.Barrier(10, 1, ImageLayout.General, ImageLayout.TransferSrcOptimal, uint.MaxValue, uint.MaxValue); j.Barrier(10, 1, ImageLayout.TransferSrcOptimal, ImageLayout.General, uint.MaxValue, uint.MaxValue); j.End(10); j.Submit([10], Result.Success); Expect(j, ImageLayout.General); });
        Test("pool-destruction-releases-metadata", j => { Record(j, 10, ImageLayout.General, ImageLayout.TransferSrcOptimal); j.FreePool(100); j.RemoveImage(1); if (j.Buffers.Count != 0 || j.Images.Count != 0) throw new Exception("Metadata retained."); });
        return new { status = "PASS", scope = "managed-journal-only; no fake native driver results", cases = passed };
    }
}
