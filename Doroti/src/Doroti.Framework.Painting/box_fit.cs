// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_fit.dart
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

public enum BoxFit
{
    fill,
    contain,
    cover,
    fitWidth,
    fitHeight,
    none,
    scaleDown
}

public class FittedSizes
{
    public virtual Size source { get; private set; } = default!;
    public virtual Size destination { get; private set; } = default!;

    public FittedSizes(Size source, Size destination)
    {
        this.source = source;
        this.destination = destination;
    }

}

public static partial class Box_fitLibrary
{
    public static FittedSizes applyBoxFit(BoxFit fit, Size inputSize, Size outputSize)
    {
        if (((((inputSize.height <= 0.0) || (inputSize.width <= 0.0)) || (outputSize.height <= 0.0)) || (outputSize.width <= 0.0)))
        {
            return new FittedSizes(Size.zero, Size.zero);
        }
        global::Doroti.Ui.Size sourceSize = default!;
        global::Doroti.Ui.Size destinationSize = default!;
        switch (fit)
        {
            case BoxFit.fill:
                {
                    sourceSize = inputSize;
                    destinationSize = outputSize;
                    break;
                }
            case BoxFit.contain:
                {
                    sourceSize = inputSize;
                    if (((outputSize.width / outputSize.height) > (sourceSize.width / sourceSize.height)))
                    {
                        destinationSize = new global::Doroti.Ui.Size(((sourceSize.width * outputSize.height) / sourceSize.height), outputSize.height);
                    }
                    else
                    {
                        destinationSize = new global::Doroti.Ui.Size(outputSize.width, ((sourceSize.height * outputSize.width) / sourceSize.width));
                    }
                    break;
                }
            case BoxFit.cover:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize = new global::Doroti.Ui.Size(inputSize.width, ((inputSize.width * outputSize.height) / outputSize.width));
                    }
                    else
                    {
                        sourceSize = new global::Doroti.Ui.Size(((inputSize.height * outputSize.width) / outputSize.height), inputSize.height);
                    }
                    destinationSize = outputSize;
                    break;
                }
            case BoxFit.fitWidth:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize = new global::Doroti.Ui.Size(inputSize.width, ((inputSize.width * outputSize.height) / outputSize.width));
                        destinationSize = outputSize;
                    }
                    else
                    {
                        sourceSize = inputSize;
                        destinationSize = new global::Doroti.Ui.Size(outputSize.width, ((sourceSize.height * outputSize.width) / sourceSize.width));
                    }
                    break;
                }
            case BoxFit.fitHeight:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize = inputSize;
                        destinationSize = new global::Doroti.Ui.Size(((sourceSize.width * outputSize.height) / sourceSize.height), outputSize.height);
                    }
                    else
                    {
                        sourceSize = new global::Doroti.Ui.Size(((inputSize.height * outputSize.width) / outputSize.height), inputSize.height);
                        destinationSize = outputSize;
                    }
                    break;
                }
            case BoxFit.none:
                {
                    sourceSize = new global::Doroti.Ui.Size(Math.Min(inputSize.width, outputSize.width), Math.Min(inputSize.height, outputSize.height));
                    destinationSize = sourceSize;
                    break;
                }
            case BoxFit.scaleDown:
                {
                    sourceSize = inputSize;
                    destinationSize = inputSize;
                    double aspectRatio = (inputSize.width / inputSize.height);
                    if ((destinationSize.height > outputSize.height))
                    {
                        destinationSize = new global::Doroti.Ui.Size((outputSize.height * aspectRatio), outputSize.height);
                    }
                    if ((destinationSize.width > outputSize.width))
                    {
                        destinationSize = new global::Doroti.Ui.Size(outputSize.width, (outputSize.width / aspectRatio));
                    }
                    break;
                }
        }
        return new FittedSizes(sourceSize, destinationSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

