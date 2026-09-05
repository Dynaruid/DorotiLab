#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/clipboard.dart
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
        await RequireHost("setData").SetClipboardTextAsync(data.text ?? string.Empty);
    }

    public static async Future<ClipboardData?> getData(string format)
    {
        if (format != kTextPlain) return null;
        var text = await RequireHost("getData").GetClipboardTextAsync();
        return text is null ? null : new ClipboardData(text);
    }

    public static async Future<bool> hasStrings()
    {
        return await RequireHost("hasStrings").HasClipboardTextAsync();
    }

    private static IPlatformServicesHostCapability RequireHost(string operation)
    {
        var invocation = DartUiInvocation.Managed($"package:flutter/services.dart#Clipboard.{operation}");
        var dispatcher = PlatformDispatcher.instance;
        var view = dispatcher.implicitView ?? dispatcher.views.FirstOrDefault()
            ?? throw new DorotiCapabilityException(DorotiCapabilityIds.PlatformServices, null,
                invocation, "clipboard access requires an attached DorotiView");
        return view.RequireCapability<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, invocation);
    }

}
