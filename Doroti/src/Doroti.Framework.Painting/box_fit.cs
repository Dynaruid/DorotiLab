// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/box_fit.dart
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public enum BoxFit
{
    fill,
    contain,
    cover,
    fitWidth,
    fitHeight,
    none,
    scaleDown,
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
        if (
            (inputSize.height <= 0.0)
            || (inputSize.width <= 0.0)
            || (outputSize.height <= 0.0)
            || (outputSize.width <= 0.0)
        )
        {
            return new FittedSizes(Size.zero, Size.zero);
        }
        Size sourceSize = default!;
        Size destinationSize = default!;
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
                if ((outputSize.width / outputSize.height) > (sourceSize.width / sourceSize.height))
                {
                    destinationSize = new Size(
                        sourceSize.width * outputSize.height / sourceSize.height,
                        outputSize.height
                    );
                }
                else
                {
                    destinationSize = new Size(
                        outputSize.width,
                        sourceSize.height * outputSize.width / sourceSize.width
                    );
                }
                break;
            }
            case BoxFit.cover:
            {
                if ((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height))
                {
                    sourceSize = new Size(
                        inputSize.width,
                        inputSize.width * outputSize.height / outputSize.width
                    );
                }
                else
                {
                    sourceSize = new Size(
                        inputSize.height * outputSize.width / outputSize.height,
                        inputSize.height
                    );
                }
                destinationSize = outputSize;
                break;
            }
            case BoxFit.fitWidth:
            {
                if ((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height))
                {
                    sourceSize = new Size(
                        inputSize.width,
                        inputSize.width * outputSize.height / outputSize.width
                    );
                    destinationSize = outputSize;
                }
                else
                {
                    sourceSize = inputSize;
                    destinationSize = new Size(
                        outputSize.width,
                        sourceSize.height * outputSize.width / sourceSize.width
                    );
                }
                break;
            }
            case BoxFit.fitHeight:
            {
                if ((outputSize.width / outputSize.height) > (inputSize.width / inputSize.height))
                {
                    sourceSize = inputSize;
                    destinationSize = new Size(
                        sourceSize.width * outputSize.height / sourceSize.height,
                        outputSize.height
                    );
                }
                else
                {
                    sourceSize = new Size(
                        inputSize.height * outputSize.width / outputSize.height,
                        inputSize.height
                    );
                    destinationSize = outputSize;
                }
                break;
            }
            case BoxFit.none:
            {
                sourceSize = new Size(
                    Math.Min(inputSize.width, outputSize.width),
                    Math.Min(inputSize.height, outputSize.height)
                );
                destinationSize = sourceSize;
                break;
            }
            case BoxFit.scaleDown:
            {
                sourceSize = inputSize;
                destinationSize = inputSize;
                double aspectRatio = inputSize.width / inputSize.height;
                if (destinationSize.height > outputSize.height)
                {
                    destinationSize = new Size(outputSize.height * aspectRatio, outputSize.height);
                }
                if (destinationSize.width > outputSize.width)
                {
                    destinationSize = new Size(outputSize.width, outputSize.width / aspectRatio);
                }
                break;
            }
        }
        return new FittedSizes(sourceSize, destinationSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
