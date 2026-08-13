using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Widgets;

public sealed class Opacity : SingleChildRenderObjectWidget
{
    public Opacity(double opacity, Widget? child = null, Key? key = null)
        : base(child, key)
    {
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
        Value = opacity;
    }

    public double Value { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderOpacity(Value);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderOpacity)renderObject).Opacity = Value;
}

public sealed class RenderOpacity : RenderProxyBox
{
    private double _opacity;

    public RenderOpacity(double opacity) => _opacity = opacity;

    public double Opacity
    {
        get => _opacity;
        set
        {
            if (!double.IsFinite(value) || value is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (_opacity != value)
            {
                _opacity = value;
                MarkNeedsPaint();
            }
        }
    }

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(Size.Zero));
            return;
        }
        Child.Layout(Constraints, parentUsesSize: true);
        SetSize(Constraints.Constrain(Child.Size));
        ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
    }

    protected override void Paint(PaintingContext context, Offset offset)
    {
        if (Child is null || _opacity <= 0)
        {
            return;
        }
        context.PushOpacity(_opacity, nested => nested.PaintChild(Child, Offset.Zero));
    }
}

public sealed class OverlayEntry(Func<BuildContext, Widget> builder, bool opaque = false)
{
    internal Guid Id { get; } = Guid.NewGuid();

    public Func<BuildContext, Widget> Builder { get; } = builder ?? throw new ArgumentNullException(nameof(builder));

    public bool Opaque { get; } = opaque;

    public bool Mounted { get; internal set; }
}

public sealed class OverlayController : ChangeNotifier
{
    private readonly List<OverlayEntry> _entries = [];

    public IReadOnlyList<OverlayEntry> Entries => _entries.ToArray();

    public void Insert(OverlayEntry entry, OverlayEntry? above = null)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.Mounted || _entries.Contains(entry))
        {
            throw new InvalidOperationException("OverlayEntry is already mounted.");
        }
        var index = above is null ? _entries.Count : _entries.IndexOf(above) + 1;
        if (index <= 0 && above is not null)
        {
            throw new ArgumentException("The 'above' entry does not belong to this overlay.", nameof(above));
        }
        _entries.Insert(index, entry);
        entry.Mounted = true;
        NotifyListeners();
    }

    public bool Remove(OverlayEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (!_entries.Remove(entry))
        {
            return false;
        }
        entry.Mounted = false;
        NotifyListeners();
        return true;
    }

    public override void Dispose()
    {
        foreach (var entry in _entries)
        {
            entry.Mounted = false;
        }
        _entries.Clear();
        base.Dispose();
    }
}

public sealed class Overlay(OverlayController controller, Key? key = null) : StatefulWidget(key)
{
    public OverlayController Controller { get; } = controller ?? throw new ArgumentNullException(nameof(controller));

    public override State CreateState() => new OverlayState();
}

public sealed class OverlayState : State<Overlay>
{
    protected internal override void InitState() => Widget.Controller.AddListener(HandleChanged);

    protected internal override void DidUpdateWidget(Overlay oldWidget)
    {
        if (!ReferenceEquals(oldWidget.Controller, Widget.Controller))
        {
            oldWidget.Controller.RemoveListener(HandleChanged);
            Widget.Controller.AddListener(HandleChanged);
        }
    }

    protected internal override void Dispose() => Widget.Controller.RemoveListener(HandleChanged);

    public override Widget Build(BuildContext context)
    {
        var entries = Widget.Controller.Entries;
        var firstVisible = 0;
        for (var index = entries.Count - 1; index >= 0; index--)
        {
            if (entries[index].Opaque)
            {
                firstVisible = index;
                break;
            }
        }
        return new Stack(entries.Skip(firstVisible).Select(entry => entry.Builder(context)));
    }

    private void HandleChanged()
    {
        if (Mounted)
        {
            SetState(static () => { });
        }
    }
}

public sealed class ModalBarrier : StatelessWidget
{
    public ModalBarrier(Action? onDismiss = null, Color? color = null, string? semanticsLabel = null, Key? key = null)
        : base(key)
    {
        OnDismiss = onDismiss;
        Color = color ?? Color.FromArgb(128, 0, 0, 0);
        SemanticsLabel = semanticsLabel;
    }

    public Action? OnDismiss { get; }

    public Color Color { get; }

    public string? SemanticsLabel { get; }

    public override Widget Build(BuildContext context)
    {
        Widget barrier = new ColoredBox(Color);
        if (OnDismiss is not null)
        {
            barrier = new GestureDetector(OnDismiss, barrier);
        }
        return new Semantics(
            barrier,
            SemanticsRole.Dialog,
            SemanticsLabel,
            state: SemanticsState.Enabled,
            onDismiss: OnDismiss);
    }
}
