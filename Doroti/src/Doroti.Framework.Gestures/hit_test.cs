// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/hit_test.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

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
    internal object identity { get; set; } = default!;

    public HitTestEntry(T target)
    {
        this.target = target;
        identity = this;
    }

    public override string ToString() => $"{DiagnosticsLibrary.describeIdentity(this)}({target})";
    public virtual Matrix4? transform => _transform;
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
        return matrix.multiplied(rhs);
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
    __cascade.leftTranslateByDouble(offset.dx, offset.dy, 0, 1);
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
        _path = new List<HitTestEntry<HitTestTarget>>();
        _transforms = new List<Matrix4> { Matrix4.identity() };
        _localTransforms = new List<_TransformPart__hit_test>();
    }

    protected HitTestResult(HitTestResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _path = result._path;
        _transforms = result._transforms;
        _localTransforms = result._localTransforms;
    }

    public static HitTestResult CreateWrap(HitTestResult result)
    {
        var __instance = new HitTestResult();
        __instance._path = result._path;
        __instance._transforms = result._transforms;
        __instance._localTransforms = result._localTransforms;
        return __instance;
    }

    public virtual IEnumerable<HitTestEntry<HitTestTarget>> path => _path;
    internal virtual void _globalizeTransforms()
    {
        if (checked((long)_localTransforms.Count) == 0)
        {
            return;
        }
        Matrix4 last = _transforms.Last();
        foreach (_TransformPart__hit_test part in _localTransforms)
        {
            last = part.multiply(last);
            _transforms.Add(last);
        }
        _localTransforms.Clear();
    }

    internal virtual Matrix4 _lastTransform
    {
        get
        {
            _globalizeTransforms();
            DartRuntimePrimitives.Assert(() => checked((long)_localTransforms.Count) == 0);
            return _transforms.Last();
        }
    }
    public virtual void add(HitTestEntry<HitTestTarget> entry)
    {
        DartRuntimePrimitives.Assert(() => entry._transform is null);
        entry._transform = _lastTransform;
        _path.Add(entry);
    }

    public virtual void pushTransform(Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => _debugVectorMoreOrLessEquals(transform.getRow(2L), new System.Numerics.Vector4(checked(0), checked(0), checked(1), checked(0))) && _debugVectorMoreOrLessEquals(transform.getColumn(2L), new System.Numerics.Vector4(checked(0), checked(0), checked(1), checked(0))));
        _localTransforms.Add(new _MatrixTransformPart__hit_test(transform));
    }

    public virtual void pushOffset(Offset offset)
    {
        _localTransforms.Add(new _OffsetTransformPart__hit_test(offset));
    }

    public virtual void popTransform()
    {
        if (checked((long)_localTransforms.Count) != 0)
        {
            _localTransforms.removeLast();
        }
        else
        {
            _transforms.removeLast();
        }
        DartRuntimePrimitives.Assert(() => checked((long)_transforms.Count) != 0);
    }

    internal virtual bool _debugVectorMoreOrLessEquals(System.Numerics.Vector4 a, System.Numerics.Vector4 b, double epsilon = 1e-10)
    {
        var result = true;
        DartRuntimePrimitives.Assert(() =>
            {
                System.Numerics.Vector4 difference = a - b;
                result = new double[] { difference.X, difference.Y, difference.Z, difference.W }.All((component) => component.abs() < epsilon);
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"HitTestResult({((checked((long)_path.Count) == 0) ? "<empty path>" : string.Join(", ", _path))})";
    public virtual void add<T>(HitTestEntry<T> entry) where T : HitTestTarget
    {
        DartRuntimePrimitives.Assert(() => entry._transform is null);
        var compatibleEntry = new HitTestEntry<HitTestTarget>(entry.target)
        {
            _transform = _lastTransform,
            identity = entry.identity,
        };
        entry._transform = compatibleEntry._transform;
        _path.Add(compatibleEntry);
    }
}
