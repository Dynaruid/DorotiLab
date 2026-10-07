using System.Diagnostics;
using Doroti.Framework.Foundation;
using Doroti.Framework.Gestures;
using Doroti.Framework.Widgets;
using Doroti.Testing;
using Doroti.Ui;

internal static class GestureDiagnosticsRegression
{
    internal static void Run()
    {
        var planar = Matrix4.translationValues(123.456789123, -98.765432198, 0)
            * Matrix4.rotationZ(0.432109876)
            * Matrix4.diagonal3Values(0.67123456789, 0.89123456789, 1);
        var perspective = Matrix4.identity();
        perspective.setEntry(3, 2, 0.001);
        perspective = perspective * Matrix4.rotationX(0.37) * Matrix4.rotationY(-0.24)
            * Matrix4.translationValues(8.123456789, -9.876543219, 3);
        foreach (var matrix in new[]
        {
            Matrix4.identity(), planar, perspective,
            new Matrix4([0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]),
            Matrix4.diagonal3Values(1e-60, 2e-60, 3e-60),
            new Matrix4([2, 3, 1, 4, 5, 1, 0, 2, 1, 0, 3, 1, 4, 2, 1, 5]),
        })
        {
            var original = matrix.storage.ToArray();
            var inverse = Matrix4.tryInvert(matrix) ?? throw new Exception("An invertible double matrix was rejected.");
            foreach (var product in new[] { matrix * inverse, inverse * matrix })
                for (var entry = 0; entry < 16; entry++)
                    Require(Math.Abs(product.storage[entry] - (entry % 5 == 0 ? 1 : 0)) < 1e-10,
                        $"Inverse product entry {entry} lost double precision: {product.storage[entry]:R}.");
            Require(original.SequenceEqual(matrix.storage), "Inversion mutated the source matrix.");
        }
        var planarInverse = Matrix4.tryInvert(planar)!;
        var result = new HitTestResult();
        result.pushTransform(planarInverse);
        result.popTransform();
        Require(planarInverse.getRow(2) == new System.Numerics.Vector4(0, 0, 1, 0)
            && planarInverse.getColumn(2) == new System.Numerics.Vector4(0, 0, 1, 0),
            "A 2D inverse changed the untouched z axis.");
        Require(Matrix4.tryInvert(Matrix4.diagonal3Values(1, 0, 1)) is null, "Singular matrix accepted.");
        Require(Matrix4.tryInvert(Matrix4.diagonal3Values(double.NaN, 1, 1)) is null, "NaN matrix accepted.");
        Require(Matrix4.tryInvert(Matrix4.diagonal3Values(double.PositiveInfinity, 1, 1)) is null,
            "Infinite matrix accepted.");
#if DEBUG
        var invalidTransformRejected = false;
        try { result.pushTransform(Matrix4.rotationX(0.5)); }
        catch (Doroti.Runtime.AssertionError) { invalidTransformRejected = true; }
        Require(invalidTransformRejected, "The hit-test invariant no longer rejects unflattened 3D transforms.");
#endif
        Console.WriteLine("PASS: double matrix inverse, pivoting, perspective, z-axis hit-test invariant and singular/nonfinite inputs.");

        Exception error;
        try { ThrowDiagnosticException(); throw new Exception("Expected diagnostic exception."); }
        catch (InvalidOperationException failure) { error = failure; }
        var details = new FlutterErrorDetails(error);
        Require(details.ToString().StartsWith("DorotiErrorDetails: Doroti framework"), "Diagnostic branding was lost.");
        Require(details.stack?.ToString().Contains(nameof(ThrowDiagnosticException)) == true,
            "Error details lost the original exception stack.");
        var explicitStack = new StackTrace();
        Require(ReferenceEquals(new FlutterErrorDetails(error, stack: explicitStack).stack, explicitStack),
            "An explicitly supplied diagnostic stack was replaced.");
        Require(!details.ToString().Contains("Flutter"), "Flutter branding remained in default diagnostics.");
        Require(new FlutterError([new ErrorSummary("diagnostic")]).diagnostics.toStringDeep().StartsWith("DorotiError"),
            "Grouped error diagnostics lost Doroti branding.");
        var printed = new List<string?>();
        var oldPrint = PrintLibrary.debugPrint;
        try
        {
            PrintLibrary.debugPrint = (message, _) => printed.Add(message);
            FlutterError.dumpErrorToConsole(details);
            FlutterError.dumpErrorToConsole(new FlutterErrorDetails(error, silent: true));
            Require(printed.Count == 1 && printed[0] == details.ToString(), "Console output/silent diagnostics changed.");
        }
        finally { PrintLibrary.debugPrint = oldPrint; }

        using var tester = new WidgetTester(new Size(100, 100));
        tester.pumpWidget(new SizedBox());
        var oldError = FlutterError.onError;
        var reported = new List<FlutterErrorDetails>();
        try
        {
            FlutterError.onError = reported.Add;
            var datum = new PointerData(tester.View.viewId, tester.Clock.Elapsed, PointerChange.add,
                PointerDeviceKind.mouse, ulong.MaxValue, 10, 10, 0, 0, 0);
            tester.View.DispatchPlatformEvent(() => PlatformDispatcher.instance.onPointerDataPacket!
                .Invoke(tester.View, new PointerDataPacket([datum])));
            Require(reported.Count == 1 && reported[0].exceptionThrown is OverflowException
                && reported[0].stack?.ToString().Contains(nameof(PointerEventConverter)) == true,
                "Pointer packet diagnostics replaced the original throw site with the catch site.");
        }
        finally { FlutterError.onError = oldError; }
        Console.WriteLine("PASS: Doroti diagnostic/console labels, silent errors, explicit stacks and original pointer exception stack.");

        var focus = new FocusNode();
        try
        {
            tester.pumpWidget(new Focus(focusNode: focus, child: new SizedBox()));
            // Native startup can replace a bootstrap branch before the first
            // frame finalizes inactive elements, while focus work is queued.
            tester.View.DispatchPlatformEvent(() =>
            {
                focus.requestFocus();
                var binding = (WidgetsFlutterBinding)WidgetsFlutterBinding.instance;
                binding.attachRootWidget(binding.wrapWithDefaultView(new SizedBox()));
                binding.buildOwner!.buildScope(binding.rootElement!);
            });
            tester.pump();
        }
        finally { focus.dispose(); }
        Console.WriteLine("PASS: queued focus notifications between branch deactivation and frame finalization.");
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void ThrowDiagnosticException() => throw new InvalidOperationException("diagnostic exception");

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
