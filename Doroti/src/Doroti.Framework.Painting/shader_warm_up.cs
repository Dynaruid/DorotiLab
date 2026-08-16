// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/shader_warm_up.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public abstract class ShaderWarmUp
{
    protected ShaderWarmUp()
    {
    }

    public virtual global::Doroti.Ui.Size size => new global::Doroti.Ui.Size(100.0, 100.0);
    public abstract Future warmUpOnCanvas(Canvas canvas);
    public async virtual Future execute()
    {
        var recorder__4062 = new global::Doroti.Ui.PictureRecorder();
        var canvas__4105 = new global::Doroti.Ui.Canvas(recorder__4062);
        await warmUpOnCanvas(canvas__4105);
        global::Doroti.Ui.Picture picture__4190 = recorder__4062.endRecording();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCaptureShaderWarmUpPicture(picture__4190));
        TimelineTask? debugShaderWarmUpTask__4297 = default!;
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugShaderWarmUpTask__4297 = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("Warm-up shader");
    return __cascade;
}))();
        }
        try
        {
            global::Doroti.Ui.Image image__4453 = await picture__4190.toImage(this.size.width.ceil(), this.size.height.ceil());
            DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCaptureShaderWarmUpImage(image__4453));
            image__4453.dispose();
        }
        finally
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugShaderWarmUpTask__4297!.finish();
            }
        }
        picture__4190.dispose();
    }

}

