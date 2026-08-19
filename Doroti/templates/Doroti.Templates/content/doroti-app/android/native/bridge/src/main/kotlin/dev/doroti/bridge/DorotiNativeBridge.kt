package dev.doroti.bridge

import android.app.Activity
import android.os.Build
import org.json.JSONObject

object DorotiNativeBridge {
    @JvmStatic
    fun platformInfo(): String = JSONObject()
        .put("platform", "Android")
        .put("osVersion", Build.VERSION.RELEASE ?: "unknown")
        .put("bridgeVersion", "1.0.0")
        .toString()

    @JvmStatic
    fun echo(value: String): String = DorotiNativeCore.echo(value)

    @JvmStatic
    fun echoOnUiThread(activity: Activity, value: String, callback: DorotiResultCallback) {
        activity.runOnUiThread { callback.onResult(echo(value)) }
    }
}
