#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/flutter_version.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public abstract class FlutterVersion
{
    public static string? version = (Environment.GetEnvironmentVariable("FLUTTER_VERSION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_VERSION") : null);
    public static string? channel = (Environment.GetEnvironmentVariable("FLUTTER_CHANNEL") is not null ? Environment.GetEnvironmentVariable("FLUTTER_CHANNEL") : null);
    public static string? gitUrl = (Environment.GetEnvironmentVariable("FLUTTER_GIT_URL") is not null ? Environment.GetEnvironmentVariable("FLUTTER_GIT_URL") : null);
    public static string? frameworkRevision = (Environment.GetEnvironmentVariable("FLUTTER_FRAMEWORK_REVISION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_FRAMEWORK_REVISION") : null);
    public static string? engineRevision = (Environment.GetEnvironmentVariable("FLUTTER_ENGINE_REVISION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_ENGINE_REVISION") : null);
    public static string? dartVersion = (Environment.GetEnvironmentVariable("FLUTTER_DART_VERSION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_DART_VERSION") : null);

    protected FlutterVersion()
    {
    }

}

