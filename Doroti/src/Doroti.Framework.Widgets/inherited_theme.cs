// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/inherited_theme.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public abstract class InheritedTheme : InheritedWidget
{
    protected InheritedTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public abstract Widget wrap(BuildContext context, Widget child);
    public static Widget captureAll(BuildContext context, Widget child, BuildContext? to = null)
    {
        return ((Widget)(object?)InheritedTheme.capture(from: context, to: to).wrap(child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CapturedThemes capture(BuildContext from, BuildContext? to)
    {
        if ((object.Equals(from, to)))
        {
            return new CapturedThemes(new List<InheritedTheme>());
        }
        var themes__4057 = new List<InheritedTheme>();
        var themeTypes__4096 = new HashSet<Type>();
        bool debugDidFindAncestor__4133 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugDidFindAncestor__4133 = (to is null);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        from.visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
if ((object.Equals(ancestor, to)))
{
    DartRuntimePrimitives.Assert(() =>
        {
            debugDidFindAncestor__4133 = true;
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    return false;
}
if (ancestor is InheritedElement { widget: InheritedTheme theme__4517 } __object4471)
{
    Type themeType__4546 = DartRuntimePrimitives.RuntimeType(theme__4517);
    if (!themeTypes__4096.Contains(themeType__4546))
    {
        themeTypes__4096.Add(themeType__4546);
        themes__4057.Add(theme__4517);
    }
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        DartRuntimePrimitives.Assert(() => debugDidFindAncestor__4133, () => (object?)"The provided `to` context must be an ancestor of the `from` context.");
        return new CapturedThemes(themes__4057);
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
        return ((Widget)(object?)new _CaptureAll__inherited_theme(themes: this._themes, child: child));
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
        Widget wrappedChild__5719 = this.child;
        foreach (InheritedTheme theme__5771 in this.themes)
        {
            wrappedChild__5719 = theme__5771.wrap(context, wrappedChild__5719);
        }
        return wrappedChild__5719;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

