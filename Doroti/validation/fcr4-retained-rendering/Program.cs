using Doroti.Framework.Rendering;
using Doroti.Ui;

VerifyCleanSubtreeUsesOneRetainedNode();
VerifyDirtySubtreeRebuildsOnlyThatScope();
VerifyEffectScopeRetainsAndReleasesItsEngineLayer();

Console.WriteLine($"FCR-4 retained rendering runtime contract: PASS (configuration={ConfigurationName()})");

static void VerifyCleanSubtreeUsesOneRetainedNode()
{
    var root = new ContainerLayer();
    var boundary = new OffsetLayer(new Offset(8, 12));
    boundary.append(PictureLayer());
    root.append(boundary);

    using var first = root.buildScene(new SceneBuilder(viewId: 9));
    Require(first.Commands.Select(command => command.Operation)
        .SequenceEqual(["offset", "picture", "pop"]),
        "initial repaint boundary records its picture and scope");

    using var idle = root.buildScene(new SceneBuilder(viewId: 9));
    Require(idle.Commands.Count == 1 && idle.Commands[0].Operation == "retained",
        "unchanged repaint boundary contributes exactly one retained node");
    Require(boundary.engineLayer is { debugDisposed: false },
        "retained engine layer remains live across an idle frame");

    root.removeAllChildren();
    root.dispose();
}

static void VerifyDirtySubtreeRebuildsOnlyThatScope()
{
    var root = new ContainerLayer();
    var boundary = new OffsetLayer();
    boundary.append(PictureLayer());
    root.append(boundary);
    var unchangedSibling = new OffsetLayer(new Offset(40, 0));
    unchangedSibling.append(PictureLayer());
    root.append(unchangedSibling);
    using var first = root.buildScene(new SceneBuilder(viewId: 10));
    var engineLayer = boundary.engineLayer ?? throw new InvalidOperationException("offset layer did not create an engine layer");
    using var idle = root.buildScene(new SceneBuilder(viewId: 10));

    boundary.offset = new Offset(20, 4);
    using var changed = root.buildScene(new SceneBuilder(viewId: 10));
    Require(changed.Commands.Select(command => command.Operation)
        .SequenceEqual(["offset", "picture", "pop", "retained"]),
        "dirty boundary records only its scope while clean sibling is retained");
    Require(ReferenceEquals(engineLayer, boundary.engineLayer) && !engineLayer.debugDisposed,
        "in-place engine-layer reuse never disposes the live handle");

    root.removeAllChildren();
    root.dispose();
    Require(engineLayer.debugDisposed, "released layer disposes its retained engine resource");
}

static void VerifyEffectScopeRetainsAndReleasesItsEngineLayer()
{
    var before = EngineLayer.debugResourceDiagnostics;
    var root = new ContainerLayer();
    var effect = new ColorFilterLayer(ColorFilter.mode(new Color(0xff336699), BlendMode.srcOver));
    effect.append(PictureLayer());
    root.append(effect);
    using var first = root.buildScene(new SceneBuilder(viewId: 11));
    using var idle = root.buildScene(new SceneBuilder(viewId: 11));
    Require(idle.Commands.Count == 1 && idle.Commands[0].Operation == "retained",
        "C1 effect scope is retained when its subtree is clean");
    var afterIdle = EngineLayer.debugResourceDiagnostics;
    Require(afterIdle.RetainedSnapshots > before.RetainedSnapshots &&
            afterIdle.RetainedReuses > before.RetainedReuses,
        "retained snapshot and reuse counters advance");

    root.removeAllChildren();
    root.dispose();
    var afterRelease = EngineLayer.debugResourceDiagnostics;
    Require(afterRelease.ActiveEngineLayers == before.ActiveEngineLayers,
        "retained engine-layer counter returns to its baseline after release");
}

static PictureLayer PictureLayer()
{
    var picture = new Picture([new PathCommand("drawColor", [0xff000000])]);
    return new PictureLayer(Rect.fromLTWH(0, 0, 32, 32)) { picture = picture };
}

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
