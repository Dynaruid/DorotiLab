// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/flavor.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public static partial class FlavorLibrary
{
    public static string? appFlavor = ((Environment.GetEnvironmentVariable("FLUTTER_APP_FLAVOR") != "") ? Environment.GetEnvironmentVariable("FLUTTER_APP_FLAVOR") : null);
}

