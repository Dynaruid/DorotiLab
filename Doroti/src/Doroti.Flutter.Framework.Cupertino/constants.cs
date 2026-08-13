// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/constants.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class ConstantsLibrary
{
    public static double kMinInteractiveDimensionCupertino = 44.0;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoFocusColorBrightness = 0.69;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoFocusColorOpacity = 0.8;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoFocusColorSaturation = 0.835;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoButtonTintedOpacityDark = 0.26;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoButtonTintedOpacityLight = 0.12;
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoButtonDefaultIconSize = 20.0;
}

public static partial class ConstantsLibrary
{
    public static DartMap<CupertinoButtonSize, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> kCupertinoButtonPadding = new DartMap<CupertinoButtonSize, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry> { [CupertinoButtonSize.small] = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 6, horizontal: 12)), [CupertinoButtonSize.medium] = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 10, horizontal: 15)), [CupertinoButtonSize.large] = ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: 16, horizontal: 20)) };
}

public static partial class ConstantsLibrary
{
    public static DartMap<CupertinoButtonSize, global::Doroti.Generated.Framework.Painting.BorderRadius> kCupertinoButtonSizeBorderRadius = new DartMap<CupertinoButtonSize, global::Doroti.Generated.Framework.Painting.BorderRadius> { [CupertinoButtonSize.small] = global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(40)), [CupertinoButtonSize.medium] = global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(40)), [CupertinoButtonSize.large] = global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular(12)) };
}

public static partial class ConstantsLibrary
{
    public static DartMap<CupertinoButtonSize, double> kCupertinoButtonMinSize = new DartMap<CupertinoButtonSize, double> { [CupertinoButtonSize.small] = 28, [CupertinoButtonSize.medium] = 32, [CupertinoButtonSize.large] = 44 };
}

public static partial class ConstantsLibrary
{
    public static double kCupertinoButtonTapMoveSlop = 70.0;
}
