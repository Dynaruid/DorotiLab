namespace Doroti.Ui;

public enum PenFieldSupport { Unknown, Unsupported, Supported }
public enum PenOrientationReference { Unknown, BarrelRotation, ScreenAzimuth }
public sealed record PenMeasurementSupport(PenFieldSupport Pressure = PenFieldSupport.Unknown,
    PenFieldSupport Tilt = PenFieldSupport.Unknown, PenFieldSupport Orientation = PenFieldSupport.Unknown,
    PenOrientationReference OrientationReference = PenOrientationReference.Unknown);

/// <summary>Pressure in [0,1], tilt from the screen normal in [0,pi/2], orientation in radians.
/// Barrel rotation and screen azimuth remain explicitly distinct; coordinates/DPR do not change these units.</summary>
public static class PenMeasurements
{
    public static double Pressure(double value) => double.IsFinite(value) ? Math.Clamp(value, 0, 1) : 0;
    public static double Radians(double degrees) => double.IsFinite(degrees) ? degrees * Math.PI / 180 : 0;
    public static double Orientation(double radians) => double.IsFinite(radians)
        ? (radians % Math.Tau + Math.Tau) % Math.Tau : 0;
    public static double Tilt(double radians) => double.IsFinite(radians) ? Math.Clamp(radians, 0, Math.PI / 2) : 0;
    public static double FromAltitude(double radians) => Tilt(Math.PI / 2 - radians);
    public static double FromAxes(double xDegrees, double yDegrees)
    {
        var x = Radians(Math.Clamp(double.IsFinite(xDegrees) ? xDegrees : 0, -90, 90));
        var y = Radians(Math.Clamp(double.IsFinite(yDegrees) ? yDegrees : 0, -90, 90));
        var cx = Math.Cos(x); var cy = Math.Cos(y);
        return Tilt(Math.Atan2(Math.Sqrt(Math.Pow(Math.Sin(x) * cy, 2) + Math.Pow(Math.Sin(y) * cx, 2)), cx * cy));
    }
}
