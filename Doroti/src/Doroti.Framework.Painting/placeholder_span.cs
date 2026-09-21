// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/placeholder_span.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public abstract class PlaceholderSpan : InlineSpan
{
    public const long placeholderCodeUnit = 65532L;
    public virtual PlaceholderAlignment alignment { get; private set; } = default!;
    public virtual TextBaseline? baseline { get; private set; }

    protected PlaceholderSpan(
        PlaceholderAlignment alignment = PlaceholderAlignment.bottom,
        TextBaseline? baseline = null,
        TextStyle? style = null
    )
        : base(style: style)
    {
        this.alignment = alignment;
        this.baseline = baseline;
    }

    public override void computeToPlainText(
        StringBuffer buffer,
        bool includeSemanticsLabels = true,
        bool includePlaceholders = true
    )
    {
        if (includePlaceholders)
        {
            buffer.writeCharCode(placeholderCodeUnit);
        }
    }

    public override void computeSemanticsInformation(
        List<InlineSpanSemanticsInformation> collector,
        Locale? inheritedLocale = null,
        bool inheritedSpellOut = false
    )
    {
        collector.Add(InlineSpanSemanticsInformation.placeholder);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new EnumProperty<PlaceholderAlignment>("alignment", alignment, defaultValue: null)
        );
        properties.add(new EnumProperty<TextBaseline>("baseline", baseline, defaultValue: null));
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => false);
        return base.debugAssertIsValid();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
