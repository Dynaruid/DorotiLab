#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/live_text.dart
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

public abstract class LiveText
{
    public static async Future<bool> isLiveTextInputAvailable()
    {
        bool supportLiveTextInput = (await SystemChannels.platform.invokeMethod<bool?>("LiveText.isLiveTextInputAvailable") ?? false);
        return supportLiveTextInput;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future startLiveTextInput()
    {
        await SystemChannels.textInput.invokeMethod<object?>("TextInput.startLiveTextInput");
    }

}

