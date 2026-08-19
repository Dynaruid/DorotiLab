package dev.doroti.bridge;

import android.app.Activity;
import android.os.Build;
import org.json.JSONException;
import org.json.JSONObject;

public final class DorotiNativeBridge {
    private DorotiNativeBridge() { }

    public static String platformInfo() {
        try {
            return new JSONObject()
                .put("platform", "Android")
                .put("osVersion", Build.VERSION.RELEASE == null ? "unknown" : Build.VERSION.RELEASE)
                .put("bridgeVersion", "1.0.0")
                .toString();
        } catch (JSONException exception) {
            throw new IllegalStateException("Unable to create the Doroti native platformInfo payload.", exception);
        }
    }

    public static String echo(String value) {
        return DorotiNativeCore.echo(value);
    }

    public static void echoOnUiThread(
        Activity activity,
        String value,
        DorotiResultCallback callback) {
        if (activity == null) throw new IllegalArgumentException("activity");
        if (callback == null) throw new IllegalArgumentException("callback");
        activity.runOnUiThread(() -> callback.onResult(echo(value)));
    }
}
