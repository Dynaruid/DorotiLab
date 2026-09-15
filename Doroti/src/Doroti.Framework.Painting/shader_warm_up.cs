// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/shader_warm_up.dart
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
        var recorder = new global::Doroti.Ui.PictureRecorder();
        var canvas = new global::Doroti.Ui.Canvas(recorder);
        await warmUpOnCanvas(canvas);
        global::Doroti.Ui.Picture picture = recorder.endRecording();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCaptureShaderWarmUpPicture(picture));
        TimelineTask? debugShaderWarmUpTask = default!;
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            debugShaderWarmUpTask = ((Func<TimelineTask>)(() =>
{
    var __cascade = new TimelineTask();
    __cascade.start("Warm-up shader");
    return __cascade;
}))();
        }
        try
        {
            global::Doroti.Ui.Image image = await picture.toImage(this.size.width.ceil(), this.size.height.ceil());
            DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCaptureShaderWarmUpImage(image));
            image.dispose();
        }
        finally
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                debugShaderWarmUpTask!.finish();
            }
        }
        picture.dispose();
    }

}

