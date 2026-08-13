#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/clipboard.dart
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

public class ClipboardData
{
    public virtual string? text { get; private set; }

    public ClipboardData(string text)
    {
        this.text = text;
    }

}

public abstract class Clipboard
{
    public const string kTextPlain = "text/plain";

    public static async Future setData(ClipboardData data)
    {
        await SystemChannels.platform.invokeMethod<object?>("Clipboard.setData", new DartMap<string, object> { ["text"] = data.text });
    }

    public static async Future<ClipboardData?> getData(string format)
    {
        DartMap<string, object>? result = await SystemChannels.platform.invokeMethod<DartMap<string, object>?>("Clipboard.getData", format);
        if ((result is null))
        {
            return null;
        }
        return new ClipboardData(text: ((string?)result.GetValueOrDefault("text"))!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future<bool> hasStrings()
    {
        DartMap<string, object>? result = await SystemChannels.platform.invokeMethod<DartMap<string, object>?>("Clipboard.hasStrings", Clipboard.kTextPlain);
        if ((result is null))
        {
            return false;
        }
        return ((bool)result.GetValueOrDefault("value"));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

