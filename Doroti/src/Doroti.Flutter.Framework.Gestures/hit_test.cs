// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/hit_test.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

public interface HitTestable
{
    public void hitTest(HitTestResult result, Offset position);
    public void hitTestInView(HitTestResult result, Offset position, long viewId);
}
public interface HitTestDispatcher
{
    public void dispatchEvent(PointerEvent @event, HitTestResult result);
}

public interface HitTestTarget
{
    public void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry);
}

public abstract class NativeHitTestTarget
{
}

public class HitTestEntry<T> where T : HitTestTarget
{
    public virtual T target { get; private set; } = default!;
    internal virtual Matrix4? _transform { get; set; } = default;

    public HitTestEntry(T target)
    {
        this.target = target;
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({this.target})";
    public virtual Matrix4? transform => this._transform;
}

internal interface _TransformPart__hit_test
{
    public Matrix4 multiply(Matrix4 rhs);
}

internal class _MatrixTransformPart__hit_test : _TransformPart__hit_test
{
    public virtual Matrix4 matrix { get; private set; } = default!;

    internal _MatrixTransformPart__hit_test(Matrix4 matrix)
    {
        this.matrix = matrix;
    }

    public virtual Matrix4 multiply(Matrix4 rhs)
    {
        return this.matrix.multiplied(rhs);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _OffsetTransformPart__hit_test : _TransformPart__hit_test
{
    public virtual Offset offset { get; private set; } = default!;

    internal _OffsetTransformPart__hit_test(Offset offset)
    {
        this.offset = offset;
    }

    public virtual Matrix4 multiply(Matrix4 rhs)
    {
        return ((Func<Matrix4>)(() =>
{
    var __cascade = rhs.clone();
    __cascade.leftTranslateByDouble(this.offset.dx, this.offset.dy, 0, 1);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class HitTestResult
{
    internal virtual List<HitTestEntry<HitTestTarget>> _path { get; private set; } = default!;
    internal virtual List<Matrix4> _transforms { get; private set; } = default!;
    internal virtual List<_TransformPart__hit_test> _localTransforms { get; private set; } = default!;

    public HitTestResult()
    {
        this._path = new List<HitTestEntry<HitTestTarget>>();
        this._transforms = new List<Matrix4> { Matrix4.identity() };
        this._localTransforms = new List<_TransformPart__hit_test>();
    }

    protected HitTestResult(HitTestResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        this._path = result._path;
        this._transforms = result._transforms;
        this._localTransforms = result._localTransforms;
    }

    public static HitTestResult CreateWrap(HitTestResult result)
    {
        var __instance = new HitTestResult();
        __instance._path = ((HitTestResult)result)._path;
        __instance._transforms = ((HitTestResult)result)._transforms;
        __instance._localTransforms = ((HitTestResult)result)._localTransforms;
        return __instance;
    }

    public virtual IEnumerable<HitTestEntry<HitTestTarget>> path => this._path;
    internal virtual void _globalizeTransforms()
    {
        if ((checked((long)(this._localTransforms.Count)) == 0))
        {
            return;
        }
        Matrix4 last__5881 = this._transforms.Last();
        foreach (_TransformPart__hit_test part__5936 in this._localTransforms)
        {
            last__5881 = part__5936.multiply(last__5881);
            this._transforms.Add(last__5881);
        }
        this._localTransforms.Clear();
    }

    internal virtual Matrix4 _lastTransform
    {
        get
        {
            _globalizeTransforms();
            DartRuntimePrimitives.Assert(() => (checked((long)(this._localTransforms.Count)) == 0));
            return this._transforms.Last();
            return default!;
        }
    }
    public virtual void add(HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => (((HitTestEntry<HitTestTarget>)entry)._transform is null));
        entry._transform = this._lastTransform;
        this._path.Add(entry);
    }

    public virtual void pushTransform(Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (_debugVectorMoreOrLessEquals(transform.getRow(2L), new global::System.Numerics.Vector4(checked((float)0), checked((float)0), checked((float)1), checked((float)0))) && _debugVectorMoreOrLessEquals(transform.getColumn(2L), new global::System.Numerics.Vector4(checked((float)0), checked((float)0), checked((float)1), checked((float)0)))));
        this._localTransforms.Add(new _MatrixTransformPart__hit_test(transform));
    }

    public virtual void pushOffset(Offset offset)
    {
        this._localTransforms.Add(new _OffsetTransformPart__hit_test(offset));
    }

    public virtual void popTransform()
    {
        if ((checked((long)(this._localTransforms.Count)) != 0))
        {
            this._localTransforms.removeLast();
        }
        else
        {
            this._transforms.removeLast();
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._transforms.Count)) != 0));
    }

    internal virtual bool _debugVectorMoreOrLessEquals(global::System.Numerics.Vector4 a, global::System.Numerics.Vector4 b, double epsilon = 1e-10)
    {
        var result__11222 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                global::System.Numerics.Vector4 difference__11273 = (a - b);
                result__11222 = new double[] { difference__11273.X, difference__11273.Y, difference__11273.Z, difference__11273.W }.All(((component) => (component.abs() < epsilon)));
                return true;
            });
        return result__11222;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"HitTestResult({((checked((long)(this._path.Count)) == 0) ? "<empty path>" : string.Join(", ", this._path))})";
    public virtual void add<T>(HitTestEntry<T> entry) where T : HitTestTarget
    {
        DartRuntimePrimitives.Assert(() => entry._transform is null);
        var compatibleEntry = new HitTestEntry<HitTestTarget>(entry.target)
        {
            _transform = _lastTransform,
        };
        entry._transform = compatibleEntry._transform;
        _path.Add(compatibleEntry);
    }
}
