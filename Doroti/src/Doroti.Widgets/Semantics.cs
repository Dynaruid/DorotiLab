using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Widgets;

public sealed class Semantics : SingleChildRenderObjectWidget
{
    public Semantics(
        Widget? child = null,
        SemanticsRole role = SemanticsRole.Generic,
        string? label = null,
        string? value = null,
        SemanticsState state = SemanticsState.Enabled,
        Action? onTap = null,
        Action? onFocus = null,
        Action? onDismiss = null,
        Key? key = null)
        : base(child, key)
    {
        Role = role;
        Label = label;
        Value = value;
        State = state;
        OnTap = onTap;
        OnFocus = onFocus;
        OnDismiss = onDismiss;
    }

    public SemanticsRole Role { get; }

    public string? Label { get; }

    public string? Value { get; }

    public SemanticsState State { get; }

    public Action? OnTap { get; }

    public Action? OnFocus { get; }

    public Action? OnDismiss { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderSemantics(this);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) => ((RenderSemantics)renderObject).Configuration = this;
}

public sealed class RenderSemantics : RenderProxyBox
{
    public RenderSemantics(Semantics configuration) => Configuration = configuration;

    public Semantics Configuration { get; set; }

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.IsSemanticBoundary = true;
        configuration.Role = Configuration.Role;
        configuration.Label = Configuration.Label;
        configuration.Value = Configuration.Value;
        configuration.State = Configuration.State;
        if (Configuration.OnTap is not null)
        {
            configuration.On(SemanticsAction.Tap, Configuration.OnTap);
        }
        if (Configuration.OnFocus is not null)
        {
            configuration.On(SemanticsAction.Focus, Configuration.OnFocus);
        }
        if (Configuration.OnDismiss is not null)
        {
            configuration.On(SemanticsAction.Dismiss, Configuration.OnDismiss);
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
}
