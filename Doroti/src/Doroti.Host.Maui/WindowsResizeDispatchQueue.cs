#if WINDOWS
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Doroti.Host.Maui;

/// <summary>
/// Only Doroti's Composition calls may run inside a native resize rendezvous.
/// Pumping the Win32/Dispatcher queue here would re-enter input, layout and close.
/// </summary>
internal sealed class WindowsResizeDispatchQueue(Action<Action> dispatch) : IDisposable
{
    private sealed class Work(Action action, long generation)
    {
        internal readonly Action Action = action;
        internal readonly long Generation = generation;
        internal readonly TaskCompletionSource Completion = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        internal int Claimed;
    }

    private readonly ConcurrentQueue<Work> _work = new();
    private readonly AutoResetEvent _signal = new(false);
    private long _deferredGeneration;
    private long _readyGeneration;
    private long _committedGeneration;
    private long _failedGeneration;

    internal void Invoke(Action action, long generation = 0)
    {
        var work = new Work(action, generation);
        _work.Enqueue(work);
        if (generation != 0)
        {
            Volatile.Write(ref _readyGeneration, generation);
        }
        _signal.Set();
        try
        {
            dispatch(Pump);
            work.Completion.Task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }
        catch
        {
            // Cancel an unclaimed callback. If the UI already owns the COM call,
            // its caller cannot tear down resources until that call returns.
            if (Interlocked.CompareExchange(ref work.Claimed, 1, 0) != 0)
            {
                work.Completion.Task.GetAwaiter().GetResult();
            }
            throw;
        }
    }

    // All methods below are called by the window's UI thread only.
    internal void BeginResize(long generation, bool deferCommit) =>
        _deferredGeneration = deferCommit ? generation : 0;

    internal void EndResize()
    {
        _deferredGeneration = 0;
        Pump();
    }

    internal bool WaitForFrame(long generation, bool prepareOnly, TimeSpan timeout)
    {
        var started = Stopwatch.GetTimestamp();
        while (true)
        {
            Pump();
            if (_failedGeneration == generation)
            {
                return false;
            }
            if (
                _committedGeneration == generation
                || (prepareOnly && Volatile.Read(ref _readyGeneration) == generation)
            )
            {
                return true;
            }
            var remaining = timeout - Stopwatch.GetElapsedTime(started);
            if (remaining <= TimeSpan.Zero)
            {
                return false;
            }
            // CLR STA waits can pump COM/window messages, re-entering the
            // geometry transaction. A kernel wait only wakes our own mailbox.
            var result = WaitForSingleObject(
                _signal.SafeWaitHandle,
                checked((uint)Math.Ceiling(remaining.TotalMilliseconds))
            );
            if (result != 0 && result != 258)
            {
                throw new InvalidOperationException("Native resize mailbox wait failed.");
            }
        }
    }

    private void Pump()
    {
        var count = _work.Count;
        while (count-- > 0 && _work.TryDequeue(out var work))
        {
            if (work.Generation != 0 && work.Generation == _deferredGeneration)
            {
                _work.Enqueue(work);
                continue;
            }
            if (Interlocked.CompareExchange(ref work.Claimed, 1, 0) != 0)
            {
                continue;
            }
            try
            {
                work.Action();
                if (work.Generation != 0)
                {
                    _committedGeneration = work.Generation;
                }
                work.Completion.TrySetResult();
            }
            catch (Exception exception)
            {
                _failedGeneration = work.Generation;
                work.Completion.TrySetException(exception);
            }
        }
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint WaitForSingleObject(
        Microsoft.Win32.SafeHandles.SafeWaitHandle handle,
        uint milliseconds
    );

    public void Dispose() => _signal.Dispose();
}
#endif
