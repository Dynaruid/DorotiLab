// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/live_text.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public abstract class LiveText
{
    public static async Future<bool> isLiveTextInputAvailable()
    {
        bool supportLiveTextInput =
            await SystemChannels.platform.invokeMethod<bool?>("LiveText.isLiveTextInputAvailable")
            ?? false;
        return supportLiveTextInput;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static async Future startLiveTextInput()
    {
        await SystemChannels.textInput.invokeMethod<object?>("TextInput.startLiveTextInput");
    }
}
