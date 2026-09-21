// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_theme.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class InheritedTheme : InheritedWidget
{
    protected InheritedTheme(Key? key = null, Widget child = default!)
        : base(key: key, child: child) { }

    public abstract Widget wrap(BuildContext context, Widget child);

    public static Widget captureAll(BuildContext context, Widget child, BuildContext? to = null)
    {
        return capture(from: context, to: to).wrap(child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CapturedThemes capture(BuildContext from, BuildContext? to)
    {
        if (Equals(from, to))
        {
            return new CapturedThemes(new List<InheritedTheme>());
        }
        var themes = new List<InheritedTheme>();
        var themeTypes = new HashSet<Type>();
        bool debugDidFindAncestor = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            debugDidFindAncestor = to is null;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        from.visitAncestorElements(
            (ancestor) =>
            {
                if (Equals(ancestor, to))
                {
                    DartRuntimePrimitives.Assert(() =>
                    {
                        debugDidFindAncestor = true;
                        return true;
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    });
                    return false;
                }
                if (ancestor is InheritedElement { widget: InheritedTheme theme } __object4471)
                {
                    Type themeType = DartRuntimePrimitives.RuntimeType(theme);
                    if (!themeTypes.Contains(themeType))
                    {
                        themeTypes.Add(themeType);
                        themes.Add(theme);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        DartRuntimePrimitives.Assert(
            () => debugDidFindAncestor,
            () => (object?)"The provided `to` context must be an ancestor of the `from` context."
        );
        return new CapturedThemes(themes);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CapturedThemes
{
    internal virtual List<InheritedTheme> _themes { get; private set; } = default!;

    public CapturedThemes(List<InheritedTheme> _themes)
    {
        this._themes = _themes;
    }

    public virtual Widget wrap(Widget child)
    {
        return new _CaptureAll__inherited_theme(themes: _themes, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _CaptureAll__inherited_theme : StatelessWidget
{
    public virtual List<InheritedTheme> themes { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _CaptureAll__inherited_theme(List<InheritedTheme> themes, Widget child)
    {
        this.themes = themes;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        Widget wrappedChild = child;
        foreach (InheritedTheme theme in themes)
        {
            wrappedChild = theme.wrap(context, wrappedChild);
        }
        return wrappedChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
