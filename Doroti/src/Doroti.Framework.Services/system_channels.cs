#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_channels.dart
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

public abstract class SystemChannels
{
    public static MethodChannel navigation = new OptionalMethodChannel("flutter/navigation", new JSONMethodCodec());
    public static MethodChannel backGesture = new OptionalMethodChannel("flutter/backgesture");
    public static MethodChannel platform = new OptionalMethodChannel("flutter/platform", new JSONMethodCodec());
    public static OptionalMethodChannel statusBar = new OptionalMethodChannel("flutter/status_bar", new JSONMethodCodec());
    public static MethodChannel processText = new OptionalMethodChannel("flutter/processtext");
    public static MethodChannel textInput = new OptionalMethodChannel("flutter/textinput", new JSONMethodCodec());
    public static MethodChannel scribe = new OptionalMethodChannel("flutter/scribe", new JSONMethodCodec());
    public static MethodChannel spellCheck = new OptionalMethodChannel("flutter/spellcheck");
    public static MethodChannel undoManager = new OptionalMethodChannel("flutter/undomanager", new JSONMethodCodec());
    public static BasicMessageChannel<object?> keyEvent = new BasicMessageChannel<object?>("flutter/keyevent", new JSONMessageCodec());
    public static BasicMessageChannel<string?> lifecycle = new BasicMessageChannel<string?>("flutter/lifecycle", new StringCodec());
    public static BasicMessageChannel<object?> system = new BasicMessageChannel<object?>("flutter/system", new JSONMessageCodec());
    public static BasicMessageChannel<object?> accessibility = new BasicMessageChannel<object?>("flutter/accessibility", new StandardMessageCodec());
    public static MethodChannel platform_views = new MethodChannel("flutter/platform_views");
    public static MethodChannel platform_views_2 = new MethodChannel("flutter/platform_views_2");
    public static MethodChannel skia = new MethodChannel("flutter/skia", new JSONMethodCodec());
    public static MethodChannel mouseCursor = new OptionalMethodChannel("flutter/mousecursor");
    public static MethodChannel restoration = new OptionalMethodChannel("flutter/restoration");
    public static MethodChannel deferredComponent = new OptionalMethodChannel("flutter/deferredcomponent");
    public static MethodChannel localization = new OptionalMethodChannel("flutter/localization", new JSONMethodCodec());
    public static MethodChannel menu = new OptionalMethodChannel("flutter/menu");
    public static MethodChannel contextMenu = new OptionalMethodChannel("flutter/contextmenu", new JSONMethodCodec());
    public static MethodChannel keyboard = new OptionalMethodChannel("flutter/keyboard");
    public static MethodChannel sensitiveContent = new OptionalMethodChannel("flutter/sensitivecontent");

}

