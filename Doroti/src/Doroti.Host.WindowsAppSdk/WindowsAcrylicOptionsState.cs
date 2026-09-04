using System.Text;
using System.Text.Json;
using Doroti.Ui;
using Microsoft.UI.Composition.SystemBackdrops;

namespace Doroti.Host.WindowsAppSdk;

internal interface IWindowsAcrylicPresenter
{
    bool AcrylicEnabled { get; }
    void ApplySystemBrightness(Brightness brightness);
    ValueTask<ReadOnlyMemory<byte>?> HandleRuntimeMessageAsync(
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken);
    AcrylicPresenterSnapshot Snapshot();
}

/// <summary>
/// Serializes the view-scoped Acrylic option channel. The caller owns the
/// actual system-backdrop target and invokes <see cref="Attach"/> with an
/// apply delegate after that target has been attached to the HWND.
/// </summary>
internal sealed class WindowsAcrylicOptionsState : IDisposable
{
    private readonly object _optionGate = new();
    private readonly ManualResetEventSlim _attached = new();
    private WindowBackdropOptions _options;
    private Brightness _systemBrightness;
    private Action<WindowBackdropOptions, Brightness>? _apply;
    private PendingOption? _pendingOption;
    private bool _optionApplying;
    private bool _disposed;
    private long _nextOptionRevision;
    private long _appliedOptionRevisions;
    private long _supersededOptionRevisions;
    private long _failedOptionRevisions;

    internal WindowsAcrylicOptionsState(
        WindowBackdropOptions options,
        Brightness systemBrightness)
    {
        _options = ValidateOptions(options);
        _systemBrightness = systemBrightness;
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 26100))
            throw new PlatformNotSupportedException(
                "Vulkan experimentalAcrylic requires Windows 11 24H2 build 26100 or newer.");
    }

    internal WindowBackdropOptions Options => _options;
    internal Brightness SystemBrightness => _systemBrightness;
    internal long AcceptedOptionRevisions => Interlocked.Read(ref _nextOptionRevision);
    internal long AppliedOptionRevisions => Interlocked.Read(ref _appliedOptionRevisions);
    internal long SupersededOptionRevisions => Interlocked.Read(ref _supersededOptionRevisions);
    internal long FailedOptionRevisions => Interlocked.Read(ref _failedOptionRevisions);
    internal string EffectiveTheme => ResolveTheme(_options.theme, _systemBrightness).ToString();

    internal void Attach(Action<WindowBackdropOptions, Brightness> apply)
    {
        ArgumentNullException.ThrowIfNull(apply);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_optionGate)
        {
            if (_apply is not null)
                throw new InvalidOperationException("The Vulkan Acrylic option target is already attached.");
            _apply = apply;
            _attached.Set();
        }
        try
        {
            apply(_options, _systemBrightness);
        }
        catch
        {
            lock (_optionGate)
            {
                _apply = null;
                _attached.Reset();
            }
            throw;
        }
    }

    internal void Detach()
    {
        lock (_optionGate)
        {
            _apply = null;
            _attached.Reset();
        }
    }

    internal void ApplySystemBrightness(Brightness brightness)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _systemBrightness = brightness;
        Action<WindowBackdropOptions, Brightness>? apply;
        lock (_optionGate) apply = _apply;
        if (_options.theme == WindowBackdropTheme.system)
            apply?.Invoke(_options, _systemBrightness);
    }

    internal ValueTask<ReadOnlyMemory<byte>?> HandleRuntimeMessageAsync(
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken,
        Func<AcrylicPresenterSnapshot> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        cancellationToken.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (data is null || data.Value.IsEmpty)
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapshot())));

        WindowBackdropOptions options;
        try
        {
            var request = JsonSerializer.Deserialize<RuntimeOptionRequest>(
                data.Value.Span,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("The Acrylic option request is empty.");
            options = ValidateOptions(request.ApplyTo(_options));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new { status = "failed", error = exception.Message })));
        }

        var revision = Interlocked.Increment(ref _nextOptionRevision);
        var completion = new TaskCompletionSource<ReadOnlyMemory<byte>?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        PendingOption? start = null;
        lock (_optionGate)
        {
            var pending = new PendingOption(revision, options, completion);
            if (!_optionApplying)
            {
                _optionApplying = true;
                start = pending;
            }
            else
            {
                if (_pendingOption is { } superseded)
                {
                    _supersededOptionRevisions++;
                    superseded.Completion.TrySetResult(SerializeTerminal(
                        superseded.Revision, "superseded", superseded.Options));
                }
                _pendingOption = pending;
            }
        }
        if (start is not null)
            ThreadPool.QueueUserWorkItem(static value =>
            {
                var tuple = ((WindowsAcrylicOptionsState Owner, PendingOption Item))value!;
                tuple.Owner.ApplyOptionLoop(tuple.Item);
            }, (this, start));
        return new ValueTask<ReadOnlyMemory<byte>?>(
            completion.Task.WaitAsync(cancellationToken));
    }

    private void ApplyOptionLoop(PendingOption current)
    {
        while (true)
        {
            try
            {
                if (!_attached.Wait(TimeSpan.FromSeconds(15)))
                    throw new TimeoutException(
                        "The Vulkan Acrylic system-backdrop target did not attach in time.");
                Action<WindowBackdropOptions, Brightness> apply;
                lock (_optionGate)
                    apply = _apply ?? throw new InvalidOperationException(
                        "The Vulkan Acrylic system-backdrop target is unavailable.");
                apply(current.Options, _systemBrightness);
                _options = current.Options;
                Interlocked.Increment(ref _appliedOptionRevisions);
                current.Completion.TrySetResult(SerializeTerminal(
                    current.Revision, "applied", current.Options));
            }
            catch (Exception exception)
            {
                Interlocked.Increment(ref _failedOptionRevisions);
                current.Completion.TrySetResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    revision = current.Revision,
                    status = "failed",
                    error = exception.Message,
                })));
            }
            lock (_optionGate)
            {
                if (_pendingOption is null)
                {
                    _optionApplying = false;
                    return;
                }
                current = _pendingOption;
                _pendingOption = null;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        PendingOption? pending;
        lock (_optionGate)
        {
            _apply = null;
            pending = _pendingOption;
            _pendingOption = null;
            _attached.Set();
        }
        pending?.Completion.TrySetResult(SerializeTerminal(
            pending.Revision, "closed", pending.Options));
        _attached.Dispose();
    }

    private static ReadOnlyMemory<byte> SerializeTerminal(
        long revision, string status, WindowBackdropOptions options) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            revision,
            status,
            kind = options.acrylicKind.ToString(),
            theme = options.theme.ToString(),
            tintColor = options.tintColor?.value,
            options.tintOpacity,
            options.luminosityOpacity,
        }));

    private static WindowBackdropOptions ValidateOptions(WindowBackdropOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.mode != WindowBackdropMode.experimentalAcrylic)
            throw new ArgumentException(
                "A runtime Acrylic update cannot change the window topology.", nameof(options));
        if (options.tintOpacity is { } tint && (!double.IsFinite(tint) || tint is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(options.tintOpacity));
        if (options.luminosityOpacity is { } luminosity &&
            (!double.IsFinite(luminosity) || luminosity is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(options.luminosityOpacity));
        return options;
    }

    private static SystemBackdropTheme ResolveTheme(
        WindowBackdropTheme theme, Brightness brightness) => theme switch
    {
        WindowBackdropTheme.light => SystemBackdropTheme.Light,
        WindowBackdropTheme.dark => SystemBackdropTheme.Dark,
        _ => brightness == Brightness.dark
            ? SystemBackdropTheme.Dark : SystemBackdropTheme.Light,
    };

    private sealed record PendingOption(
        long Revision,
        WindowBackdropOptions Options,
        TaskCompletionSource<ReadOnlyMemory<byte>?> Completion);

    private sealed record RuntimeOptionRequest(
        string? Kind,
        string? Theme,
        uint? TintColor,
        double? TintOpacity,
        double? LuminosityOpacity)
    {
        internal WindowBackdropOptions ApplyTo(WindowBackdropOptions current) => current with
        {
            acrylicKind = Kind?.ToLowerInvariant() switch
            {
                null => current.acrylicKind,
                "default" => WindowAcrylicKind.@default,
                "base" => WindowAcrylicKind.@base,
                "thin" => WindowAcrylicKind.thin,
                _ => throw new InvalidDataException($"Unknown Acrylic kind '{Kind}'."),
            },
            theme = Theme?.ToLowerInvariant() switch
            {
                null => current.theme,
                "system" => WindowBackdropTheme.system,
                "light" => WindowBackdropTheme.light,
                "dark" => WindowBackdropTheme.dark,
                _ => throw new InvalidDataException($"Unknown Acrylic theme '{Theme}'."),
            },
            tintColor = TintColor is { } color ? new Color(color) : current.tintColor,
            tintOpacity = TintOpacity ?? current.tintOpacity,
            luminosityOpacity = LuminosityOpacity ?? current.luminosityOpacity,
        };
    }
}
