using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Common synchronous negotiation and asynchronous delivery; never calls app code on an OS callback stack.</summary>
public sealed class OsDropReceiver(OsDropSupport support, Action<Action> dispatch) : IOsDragDropHostCapability, IDisposable
{
    private readonly object _gate = new();
    private readonly HashSet<OsDropData> _payloads = [];
    private Registration? _registration;
    private bool _closed;
    public OsDropSupport Support { get; } = support with { Formats = Array.AsReadOnly(support.Formats.ToArray()) };

    public IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent)
    {
        ArgumentNullException.ThrowIfNull(onEvent);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_closed, this);
            if (!Support.CanReceive) throw new NotSupportedException("OS drop reception is unavailable on this host.");
            if (_registration is not null) throw new InvalidOperationException("This view already has an OS drop receiver.");
            return _registration = new(this, Snapshot(options), onEvent);
        }
    }

    private OsDropOptions Snapshot(OsDropOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Formats);
        if ((options.Actions & ~Support.Actions) != 0) throw new NotSupportedException("The host does not implement the requested drop actions.");
        return options with { Formats = Array.AsReadOnly(options.Formats.ToArray()) };
    }

    private OsDropAction Select(Registration? registration, OsDropOffer offer)
    {
        if (_closed || registration is null || (registration.Options.Bounds is { } bounds && !bounds.contains(offer.Position))
            || !offer.Formats.Any(format => registration.Options.Formats.Contains(format) && Support.Formats.Contains(format))) return OsDropAction.None;
        var actions = offer.SourceActions & registration.Options.Actions & Support.Actions;
        if (offer.RequestedAction != OsDropAction.None)
            return offer.RequestedAction is (OsDropAction.Copy or OsDropAction.Move or OsDropAction.Link)
                && (actions & offer.RequestedAction) == offer.RequestedAction ? offer.RequestedAction : OsDropAction.None;
        return (actions & OsDropAction.Copy) != 0 ? OsDropAction.Copy
            : (actions & OsDropAction.Link) != 0 ? OsDropAction.Link
            : (actions & OsDropAction.Move) != 0 ? OsDropAction.Move : OsDropAction.None;
    }

    public OsDropAction Hover(OsDropPhase phase, OsDropOffer offer)
    {
        if (phase is not (OsDropPhase.Enter or OsDropPhase.Over or OsDropPhase.Leave)) throw new ArgumentOutOfRangeException(nameof(phase));
        offer = offer with { Formats = Array.AsReadOnly(offer.Formats.ToArray()) };
        Registration? registration;
        OsDropAction action;
        lock (_gate) { registration = _registration; action = phase == OsDropPhase.Leave ? OsDropAction.None : Select(registration, offer); }
        if (registration is not null) Publish(registration, new(phase, offer, action));
        return action;
    }

    public OsDropAction Drop(OsDropOffer offer, Func<IReadOnlyList<string>, OsDropData> acquire)
    {
        offer = offer with { Formats = Array.AsReadOnly(offer.Formats.ToArray()) };
        Registration? registration;
        OsDropAction action;
        lock (_gate) { registration = _registration; action = Select(registration, offer); }
        if (action == OsDropAction.None) return action;
        OsDropData? data = null;
        try
        {
            IReadOnlyList<string> formats;
            lock (_gate) { formats = registration!.Options.Formats.Intersect(offer.Formats).Intersect(Support.Formats).ToArray(); }
            data = acquire(formats);
            lock (_gate)
            {
                if (_closed || _registration != registration || Select(registration, offer) != action)
                { data.Dispose(); return OsDropAction.None; }
                _payloads.Add(data);
                var retained = data;
                data.Disposed += () => { lock (_gate) _payloads.Remove(retained); };
            }
            Publish(registration!, new(OsDropPhase.Drop, offer, action, data));
            return action;
        }
        catch (Exception error)
        {
            data?.Dispose();
            Publish(registration!, new(OsDropPhase.Error, offer, OsDropAction.None, Failure:
                error is UnauthorizedAccessException ? OsDropFailure.Denied : error is InvalidDataException ? OsDropFailure.InvalidData : OsDropFailure.Failed,
                Message: error.Message));
            return OsDropAction.None;
        }
    }

    private void Publish(Registration registration, OsDropEvent item)
    {
        // Only the newest over event needs a frame. Terminal events retain ordering.
        OverBatch? batch = null;
        lock (_gate)
        {
            if (item.Phase == OsDropPhase.Over)
            {
                if (registration.PendingOver is { } pending) { pending.Event = item; return; }
                registration.PendingOver = batch = new(item);
            }
            else registration.PendingOver = null;
        }
        try
        {
            dispatch(() =>
            {
                lock (_gate)
                {
                    if (batch is not null)
                    {
                        item = batch.Event;
                        if (registration.PendingOver == batch) registration.PendingOver = null;
                    }
                    if (_closed || _registration != registration) { item.Data?.Dispose(); return; }
                }
                try { registration.Callback(item); }
                catch (Exception error) { item.Data?.Dispose(); System.Diagnostics.Trace.TraceError(error.ToString()); }
            });
        }
        catch
        {
            if (batch is not null) lock (_gate) { if (registration.PendingOver == batch) registration.PendingOver = null; }
            item.Data?.Dispose(); throw;
        }
    }

    private void Unregister(Registration registration)
    {
        OsDropData[] payloads;
        lock (_gate)
        {
            if (_registration != registration) return;
            _registration = null;
            payloads = _payloads.ToArray();
            _payloads.Clear();
        }
        DisposePayloads(payloads);
    }

    private static void DisposePayloads(IEnumerable<OsDropData> payloads)
    {
        List<Exception> errors = [];
        foreach (var payload in payloads) try { payload.Dispose(); } catch (Exception error) { errors.Add(error); }
        if (errors.Count != 0) throw new AggregateException(errors);
    }

    public void Dispose()
    {
        Registration? registration;
        lock (_gate) { if (_closed) return; _closed = true; registration = _registration; }
        registration?.Dispose();
    }

    private sealed class OverBatch(OsDropEvent item) { internal OsDropEvent Event = item; }

    private sealed class Registration(OsDropReceiver owner, OsDropOptions options, Action<OsDropEvent> callback) : IOsDropRegistration
    {
        internal OsDropOptions Options = options;
        internal Action<OsDropEvent> Callback { get; } = callback;
        internal OverBatch? PendingOver;
        public void Update(OsDropOptions value)
        {
            lock (owner._gate)
            {
                ObjectDisposedException.ThrowIf(owner._closed || owner._registration != this, this);
                Options = owner.Snapshot(value);
            }
        }
        public void Dispose() => owner.Unregister(this);
    }
}
