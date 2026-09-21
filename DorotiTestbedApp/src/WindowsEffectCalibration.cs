using Doroti.Framework.Services;
using Doroti.Ui;

internal static class WindowsEffectCalibration
{
    private static int _started;

    internal static void Start(
        DorotiView owner,
        WebViewController controller,
        string variable = "DOROTI_WINDOWS_EFFECT_CALIBRATION"
    )
    {
        var path = Environment.GetEnvironmentVariable(variable);
        if (string.IsNullOrEmpty(path) || Interlocked.Exchange(ref _started, 1) != 0)
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await controller.Ready;
                async Task Load(string style)
                {
                    await controller.ExecuteAsync(
                        new(
                            WebViewOperation.LoadHtml,
                            "<!doctype html><title>calibration</title><style>html,body{margin:0;width:100%;height:100%;"
                                + style
                                + "}</style>"
                        )
                    );
                    for (var i = 0; i < 100; i++)
                    {
                        var state = await controller.ExecuteAsync(new(WebViewOperation.State));
                        if (!state.IsLoading && state.Title == "calibration")
                        {
                            return;
                        }

                        await Task.Delay(50);
                    }
                    throw new TimeoutException("Calibration HTML did not load.");
                }
                await Load(
                    "background:linear-gradient(to right,black 0%,black 50%,white 50%,white 100%)"
                );
                async Task Stage(
                    string name,
                    double sigma,
                    double saturation = 1,
                    bool raster = false,
                    uint tint = 0
                )
                {
                    var completion = new TaskCompletionSource(
                        TaskCreationOptions.RunContinuationsAsynchronously
                    );
                    owner.DispatchPlatformEvent(() =>
                    {
                        PlatformEffectFixtureProbe.SetRasterSource?.Invoke(raster);
                        PlatformEffectFixtureProbe.SetStyle?.Invoke(
                            new(
                                Match: PlatformEffectMatchPolicy.ExactSigma,
                                ExactSigma: sigma,
                                Saturation: saturation,
                                Tint: tint
                            )
                        );
                        completion.SetResult();
                    });
                    await completion.Task;
                    await Task.Delay(500);
                    System.IO.File.WriteAllText(path + ".stage", name);
                    for (var i = 0; i < 200; i++)
                    {
                        if (
                            System.IO.File.Exists(path + ".ack")
                            && System.IO.File.ReadAllText(path + ".ack") == name
                        )
                        {
                            return;
                        }

                        await Task.Delay(50);
                    }
                    throw new TimeoutException("Calibration capture acknowledgement: " + name);
                }
                await Stage("native-zero", 0);
                await Stage("native-4", 4);
                await Stage("native-16", 16);
                await Stage("raster-4", 4, raster: true);
                await Stage("raster-16", 16, raster: true);
                await Stage("native-reset", 0);
                await Load("background:rgb(180,80,60)");
                await Stage("color-one", 0);
                await Stage("color-zero", 0, 0);
                await Stage("color-two", 0, 2);
                await Stage("color-tint", 0, 1, tint: 0x800000ff);
                await Stage("color-reset", 0);
                if (variable == "DOROTI_ANDROID_EFFECT_CALIBRATION")
                {
                    await Load(
                        "background:linear-gradient(to right,rgb(180,80,60) 0%,rgb(180,80,60) 50%,rgb(60,80,180) 50%,rgb(60,80,180) 100%)"
                    );
                    await Stage("color-edge", 16, 2);
                    await Stage("color-edge-zero", 0, 2);
                }
                System.IO.File.WriteAllText(path + ".done", "PASS");
            }
            catch (Exception error)
            {
                System.IO.File.WriteAllText(path + ".done", "FAIL " + error);
            }
        });
    }
}
