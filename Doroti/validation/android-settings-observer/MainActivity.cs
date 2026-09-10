using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Doroti.Host.Maui;

namespace Doroti.Validation.AndroidSettingsObserver;

[Activity(Name = "dev.doroti.validation.settingsobserver.MainActivity", MainLauncher = true, Exported = true)]
public sealed class MainActivity : Activity
{
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var label = new TextView(this) { Text = "Running observer regression..." };
        SetContentView(label);
        try
        {
            var calls = 0;
            using (var active = new MauiSettingsObserver(() => calls++))
            {
                active.DispatchChange(false, null);
                await Task.Delay(100);
                Require(calls == 1, "active notification must reach the owner");
            }

            // Preserve a Java reference just as a notification already held by
            // Android would. DispatchChange posts to the observer's main handler.
            var retired = new MauiSettingsObserver(() => calls++);
            var javaPeer = JNIEnv.NewGlobalRef(retired.Handle);
            try
            {
                retired.DispatchChange(false, null);
                retired.Dispose();
                await Task.Delay(100);
                Require(calls == 1, "queued notification must be canceled on disposal");

                // Force the JNI callback after the managed peer was disposed.
                // The old implementation throws MissingMethodException here (or
                // above when its uncanceled queued notification runs).
                var javaClass = JNIEnv.GetObjectClass(javaPeer);
                try
                {
                    var onChange = JNIEnv.GetMethodID(javaClass, "onChange", "(Z)V");
                    JNIEnv.CallVoidMethod(javaPeer, onChange, new JValue(false));
                }
                finally { JNIEnv.DeleteLocalRef(javaClass); }
                Require(calls == 1, "late native callback must not refresh a detached owner");
            }
            finally { JNIEnv.DeleteGlobalRef(javaPeer); }

            using (var replacement = new MauiSettingsObserver(() => calls++))
            {
                replacement.DispatchChange(false, null);
                await Task.Delay(100);
                Require(calls == 2, "replacement observer must still deliver notifications");
            }
            label.Text = "PASS: active, queued disposal, late JNI callback, reattach";
            global::Android.Util.Log.Info("DorotiObserverTest", label.Text);
        }
        catch (Exception exception)
        {
            label.Text = "FAIL: " + exception;
            global::Android.Util.Log.Error("DorotiObserverTest", label.Text);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
