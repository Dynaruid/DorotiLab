using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Immutable capabilities and identities observed before analysis. Holds no native lease.</summary>
public sealed class PlatformViewSnapshot
{
    private readonly IReadOnlyDictionary<
        PlatformViewHandle,
        IReadOnlyDictionary<PlatformViewEffects, PlatformViewSupport>
    > _instances;

    internal PlatformViewSnapshot(
        ulong owner,
        PlatformViewComposition composition,
        Dictionary<
            PlatformViewHandle,
            IReadOnlyDictionary<PlatformViewEffects, PlatformViewSupport>
        > instances
    )
    {
        OwnerViewId = owner;
        Composition = composition;
        _instances = instances.AsReadOnly();
    }

    public ulong OwnerViewId { get; }
    public PlatformViewComposition Composition { get; }

    public bool Contains(PlatformViewHandle handle) => _instances.ContainsKey(handle);

    internal void Validate(PlatformViewPlacement placement)
    {
        var effects = placement.Clip is null
            ? PlatformViewEffects.None
            : PlatformViewEffects.RectClip;
        if (
            placement.Transform.M11 != 1
            || placement.Transform.M12 != 0
            || placement.Transform.M21 != 0
            || placement.Transform.M22 != 1
        )
        {
            effects |= PlatformViewEffects.AffineTransform;
        }

        if (
            !_instances.TryGetValue(placement.Handle, out var variants)
            || !variants.TryGetValue(effects, out var support)
            || !support.Supported
            || support.Composition != Composition
            || (effects & ~support.Effects) != 0
        )
        {
            throw new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformViews,
                OwnerViewId,
                DorotiUiInvocation.Managed("PlatformViewSnapshot"),
                "Stale identity or unsupported attachment geometry."
            );
        }
    }
}
