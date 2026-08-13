#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/app_version.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public static partial class App_versionLibrary
{
    public static string? appBuildName = (Environment.GetEnvironmentVariable("FLUTTER_BUILD_NAME") is not null ? Environment.GetEnvironmentVariable("FLUTTER_BUILD_NAME") : null);
}

public static partial class App_versionLibrary
{
    public static string? appBuildNumber = (Environment.GetEnvironmentVariable("FLUTTER_BUILD_NUMBER") is not null ? Environment.GetEnvironmentVariable("FLUTTER_BUILD_NUMBER") : null);
}

