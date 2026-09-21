// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/form_row.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Form_rowLibrary
{
    internal static EdgeInsetsGeometry _kDefaultPadding = new EdgeInsetsDirectional(
        20.0,
        6.0,
        6.0,
        6.0
    );
}

public class CupertinoFormRow : StatelessWidget
{
    public virtual Widget? prefix { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Widget? helper { get; private set; }
    public virtual Widget? error { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    public CupertinoFormRow(
        Key? key = null,
        Widget child = default!,
        Widget? prefix = null,
        EdgeInsetsGeometry? padding = null,
        Widget? helper = null,
        Widget? error = null
    )
        : base(key: key)
    {
        this.child = child;
        this.prefix = prefix;
        this.padding = padding;
        this.helper = helper;
        this.error = error;
    }

    public override Widget build(BuildContext context)
    {
        CupertinoThemeData theme = CupertinoTheme.of(context);
        TextStyle textStyleLocal = theme.textTheme.textStyle.copyWith(
            color: CupertinoDynamicColor.maybeResolve(theme.textTheme.textStyle.color, context)
        );
        return new Padding(
            padding: padding ?? Form_rowLibrary._kDefaultPadding,
            child: new Column(
                children: (
                    (Func<List<Widget>>)(
                        () =>
                        {
                            var __collection5156 = new List<Widget>();
                            __collection5156.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Row(
                                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                        children: (
                                            (Func<List<Widget>>)(
                                                () =>
                                                {
                                                    var __collection5266 = new List<Widget>();
                                                    if (prefix is not null)
                                                    {
                                                        __collection5266.Add(
                                                            DartRuntimePrimitives.ConvertValue<Widget>(
                                                                new DefaultTextStyle(
                                                                    style: textStyleLocal,
                                                                    child: prefix!
                                                                )
                                                            )
                                                        );
                                                    }
                                                    __collection5266.Add(
                                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                                            new Flexible(
                                                                child: new Align(
                                                                    alignment: AlignmentDirectional.centerEnd,
                                                                    child: child
                                                                )
                                                            )
                                                        )
                                                    );
                                                    return __collection5266;
                                                }
                                            )
                                        )()
                                    )
                                )
                            );
                            if (helper is not null)
                            {
                                __collection5156.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Align(
                                            alignment: AlignmentDirectional.centerStart,
                                            child: new DefaultTextStyle(
                                                style: textStyleLocal,
                                                child: helper!
                                            )
                                        )
                                    )
                                );
                            }
                            if (error is not null)
                            {
                                __collection5156.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Align(
                                            alignment: AlignmentDirectional.centerStart,
                                            child: new DefaultTextStyle(
                                                style: new TextStyle(
                                                    color: CupertinoColors.destructiveRed,
                                                    fontWeight: FontWeight.w500
                                                ),
                                                child: error!
                                            )
                                        )
                                    )
                                );
                            }
                            return __collection5156;
                        }
                    )
                )()
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
