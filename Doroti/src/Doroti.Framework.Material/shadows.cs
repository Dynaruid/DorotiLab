// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/shadows.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public static partial class ShadowsLibrary
{
    public static DartMap<long, List<global::Doroti.Framework.Painting.BoxShadow>> kElevationToShadow => ShadowsLibrary._elevationToShadow;
}

public static partial class ShadowsLibrary
{
    internal static Color _kKeyUmbraOpacity = new global::Doroti.Ui.Color(855638016L);
}

public static partial class ShadowsLibrary
{
    internal static Color _kKeyPenumbraOpacity = new global::Doroti.Ui.Color(603979776L);
}

public static partial class ShadowsLibrary
{
    internal static Color _kAmbientShadowOpacity = new global::Doroti.Ui.Color(520093696L);
}

public static partial class ShadowsLibrary
{
    internal static DartMap<long, List<global::Doroti.Framework.Painting.BoxShadow>> _elevationToShadow = new DartMap<long, List<global::Doroti.Framework.Painting.BoxShadow>> { [0L] = new List<global::Doroti.Framework.Painting.BoxShadow>(), [1L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 2.0), blurRadius: 1.0, spreadRadius: -1.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 1.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 3.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [2L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 1.0, spreadRadius: -2.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 2.0), blurRadius: 2.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 5.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [3L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 3.0, spreadRadius: -2.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 4.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 8.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [4L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 2.0), blurRadius: 4.0, spreadRadius: -1.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 4.0), blurRadius: 5.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 10.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [6L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 5.0, spreadRadius: -1.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 6.0), blurRadius: 10.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 1.0), blurRadius: 18.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [8L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 5.0), blurRadius: 5.0, spreadRadius: -3.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 8.0), blurRadius: 10.0, spreadRadius: 1.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 14.0, spreadRadius: 2.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [9L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 5.0), blurRadius: 6.0, spreadRadius: -3.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 9.0), blurRadius: 12.0, spreadRadius: 1.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 3.0), blurRadius: 16.0, spreadRadius: 2.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [12L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 7.0), blurRadius: 8.0, spreadRadius: -4.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 12.0), blurRadius: 17.0, spreadRadius: 2.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 5.0), blurRadius: 22.0, spreadRadius: 4.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [16L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 8.0), blurRadius: 10.0, spreadRadius: -5.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 16.0), blurRadius: 24.0, spreadRadius: 2.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 6.0), blurRadius: 30.0, spreadRadius: 5.0, color: ShadowsLibrary._kAmbientShadowOpacity) }, [24L] = new List<global::Doroti.Framework.Painting.BoxShadow> { new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 11.0), blurRadius: 15.0, spreadRadius: -7.0, color: ShadowsLibrary._kKeyUmbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 24.0), blurRadius: 38.0, spreadRadius: 3.0, color: ShadowsLibrary._kKeyPenumbraOpacity), new global::Doroti.Framework.Painting.BoxShadow(offset: new global::Doroti.Ui.Offset(0.0, 9.0), blurRadius: 46.0, spreadRadius: 8.0, color: ShadowsLibrary._kAmbientShadowOpacity) } };
}
