// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/placeholder_span.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public abstract class PlaceholderSpan : InlineSpan
{
    public const long placeholderCodeUnit = 65532L;
    public virtual PlaceholderAlignment alignment { get; private set; } = default!;
    public virtual TextBaseline? baseline { get; private set; }

    protected PlaceholderSpan(PlaceholderAlignment alignment = PlaceholderAlignment.bottom, TextBaseline? baseline = null, TextStyle? style = null) : base(style: style)
    {
        this.alignment = alignment;
        this.baseline = baseline;
    }

    public override void computeToPlainText(StringBuffer buffer, bool includeSemanticsLabels = true, bool includePlaceholders = true)
    {
        if (includePlaceholders)
        {
            buffer.writeCharCode(placeholderCodeUnit);
        }
    }

    public override void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector, Locale? inheritedLocale = null, bool inheritedSpellOut = false)
    {
        collector.Add(InlineSpanSemanticsInformation.placeholder);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Ui.PlaceholderAlignment>("alignment", this.alignment, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.TextBaseline>("baseline", this.baseline, defaultValue: null));
    }

    public override bool debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => false);
        return base.debugAssertIsValid();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

