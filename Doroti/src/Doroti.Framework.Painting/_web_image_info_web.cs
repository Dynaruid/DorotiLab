// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/_web_image_info_web.dart
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

public class WebImageInfoIo : ImageInfo
{
    public virtual HTMLImageElement htmlImage { get; private set; } = default!;
    private string? __field_debugLabel = default!;
    public override string? debugLabel { get => __field_debugLabel; }

    public WebImageInfoIo(HTMLImageElement htmlImage, string? debugLabel = null)
    {
        this.htmlImage = htmlImage;
        this.__field_debugLabel = debugLabel;
    }

    public override WebImageInfoIo clone()
    {
        return new WebImageInfoIo(this.htmlImage, debugLabel: this.debugLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
    }

    public override Image image => throw new NotSupportedException("Could not create image data for this image because access to it is " + "restricted by the Same-Origin Policy.\n" + "See https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy");
    public override bool isCloneOf(ImageInfo other)
    {
        if ((other is not WebImageInfoIo))
        {
            return false;
        }
        return ((object.Equals(((WebImageInfoIo)other).htmlImage, this.htmlImage)) && (((WebImageInfoIo)other).debugLabel == this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double scale => 1.0;
    public override long sizeBytes => (((4L * this.htmlImage.naturalWidth) * this.htmlImage.naturalHeight)).toInt();
}

