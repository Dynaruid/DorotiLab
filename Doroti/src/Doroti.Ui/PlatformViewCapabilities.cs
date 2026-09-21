namespace Doroti.Ui;

public enum PlatformViewStrategyPolicy
{
    PlatformPreferred,
    RequireRequested,
}

public enum PlatformViewRepresentation
{
    NativeHierarchy,
    CompositionVisual,
    LiveTexture,
    Dom,
    Snapshot,
}

public enum PlatformViewTransport
{
    Native,
    GpuShared,
    BoundedReadback,
    CpuUpload,
    BrowserCompositor,
}

public enum PlatformViewInputPolicy
{
    DirectNative,
    GestureArena,
}

public enum PlatformCommitObservation
{
    BackendAccepted,
    CompositorAcknowledged,
    Presented,
}

public enum PlatformEffectMatchPolicy
{
    MatchCommon,
    ExactSigma,
    SolidTint,
}

/// <summary>Creation identity is independent of mutable attachment geometry. Bytes are copied on admission.</summary>
public sealed record PlatformViewDescriptor(
    string ViewType,
    ReadOnlyMemory<byte> CreationParameters = default,
    PlatformViewStrategyPolicy Strategy = PlatformViewStrategyPolicy.PlatformPreferred,
    PlatformViewComposition Composition = PlatformViewComposition.InterleavedComposition,
    PlatformViewInputPolicy Input = PlatformViewInputPolicy.DirectNative
);

/// <summary>Common visual intent; strength maps to a logical Gaussian sigma in [0, 16].
/// ExactSigma is explicitly requested. SolidTint is an accessibility alternative, not blur.</summary>
public sealed record PlatformEffectStyle(
    double Strength = .375,
    uint Tint = 0x00000000,
    double Saturation = 1,
    PlatformEffectMatchPolicy Match = PlatformEffectMatchPolicy.MatchCommon,
    double ExactSigma = 6
)
{
    public double Sigma =>
        Match == PlatformEffectMatchPolicy.SolidTint ? 0
        : Match == PlatformEffectMatchPolicy.ExactSigma ? ExactSigma
        : Strength * 16;

    public void Validate()
    {
        if (
            !Enum.IsDefined(Match)
            || !double.IsFinite(Strength)
            || Strength is < 0 or > 1
            || !double.IsFinite(Saturation)
            || Saturation is < 0 or > 2
            || !double.IsFinite(ExactSigma)
            || ExactSigma is < 0 or > 128
        )
        {
            throw new ArgumentOutOfRangeException(
                nameof(Strength),
                "Invalid platform effect intent."
            );
        }
    }
}

/// <summary>Backend limits are explicit lowering policy, not common scene parsing rules.</summary>
public sealed record PlatformEffectSupport(
    bool LiveSourceSampling = false,
    int MaximumEffects = 0,
    double MaximumSigma = 0,
    bool Saturation = false,
    string? Reason = null
)
{
    public static PlatformEffectSupport Unsupported { get; } =
        new(Reason: "This attachment cannot sample live native content.");

    public void Validate(double sigmaX, double sigmaY, int effectCount)
    {
        if (
            !LiveSourceSampling
            || effectCount > MaximumEffects
            || sigmaX > MaximumSigma
            || sigmaY > MaximumSigma
        )
        {
            throw new NotSupportedException(
                Reason ?? "Native effect exceeds this backend's sampling limits."
            );
        }
    }
}

public sealed record PlatformViewCapabilities(
    PlatformViewRepresentation Representation,
    PlatformViewTransport Transport,
    PlatformViewInputPolicy Input,
    PlatformEffectSupport Effect,
    PlatformCommitObservation Observation = PlatformCommitObservation.BackendAccepted,
    bool PhysicalAtomicDisplay = false
);

public sealed record PlatformCompositionCommitResult(
    PlatformCompositionToken Token,
    PlatformCommitObservation Observation,
    bool PhysicalAtomicDisplay = false
);
