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

namespace Doroti.Generated.Framework.Painting;

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
        global::Doroti.Ui.Size sourceSize__5855 = default!;
        global::Doroti.Ui.Size destinationSize__5867 = default!;
        switch (fit)
        {
            case BoxFit.fill:
                {
                    sourceSize__5855 = inputSize;
                    destinationSize__5867 = outputSize;
                    break;
                }
            case BoxFit.contain:
                {
                    sourceSize__5855 = inputSize;
                    if (((outputSize.width / outputSize.height) > (sourceSize__5855.width / sourceSize__5855.height)))
                    {
                        destinationSize__5867 = new global::Doroti.Ui.Size(((sourceSize__5855.width * outputSize.height) / sourceSize__5855.height), outputSize.height);
                    }
                    else
                    {
                        destinationSize__5867 = new global::Doroti.Ui.Size(outputSize.width, ((sourceSize__5855.height * outputSize.width) / sourceSize__5855.width));
                    }
                    break;
                }
            case BoxFit.cover:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize__5855 = new global::Doroti.Ui.Size(inputSize.width, ((inputSize.width * outputSize.height) / outputSize.width));
                    }
                    else
                    {
                        sourceSize__5855 = new global::Doroti.Ui.Size(((inputSize.height * outputSize.width) / outputSize.height), inputSize.height);
                    }
                    destinationSize__5867 = outputSize;
                    break;
                }
            case BoxFit.fitWidth:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize__5855 = new global::Doroti.Ui.Size(inputSize.width, ((inputSize.width * outputSize.height) / outputSize.width));
                        destinationSize__5867 = outputSize;
                    }
                    else
                    {
                        sourceSize__5855 = inputSize;
                        destinationSize__5867 = new global::Doroti.Ui.Size(outputSize.width, ((sourceSize__5855.height * outputSize.width) / sourceSize__5855.width));
                    }
                    break;
                }
            case BoxFit.fitHeight:
                {
                    if (((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height)))
                    {
                        sourceSize__5855 = inputSize;
                        destinationSize__5867 = new global::Doroti.Ui.Size(((sourceSize__5855.width * outputSize.height) / sourceSize__5855.height), outputSize.height);
                    }
                    else
                    {
                        sourceSize__5855 = new global::Doroti.Ui.Size(((inputSize.height * outputSize.width) / outputSize.height), inputSize.height);
                        destinationSize__5867 = outputSize;
                    }
                    break;
                }
            case BoxFit.none:
                {
                    sourceSize__5855 = new global::Doroti.Ui.Size(Math.Min(inputSize.width, outputSize.width), Math.Min(inputSize.height, outputSize.height));
                    destinationSize__5867 = sourceSize__5855;
                    break;
                }
            case BoxFit.scaleDown:
                {
                    sourceSize__5855 = inputSize;
                    destinationSize__5867 = inputSize;
                    double aspectRatio__8171 = (inputSize.width / inputSize.height);
                    if ((destinationSize__5867.height > outputSize.height))
                    {
                        destinationSize__5867 = new global::Doroti.Ui.Size((outputSize.height * aspectRatio__8171), outputSize.height);
                    }
                    if ((destinationSize__5867.width > outputSize.width))
                    {
                        destinationSize__5867 = new global::Doroti.Ui.Size(outputSize.width, (outputSize.width / aspectRatio__8171));
                    }
                    break;
                }
        }
        return new FittedSizes(sourceSize__5855, destinationSize__5867);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

