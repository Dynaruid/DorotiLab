// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/shadows.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ShadowsLibrary
{
    public static DartMap<long, List<BoxShadow>> kElevationToShadow => _elevationToShadow;
}

public static partial class ShadowsLibrary
{
    internal static Color _kKeyUmbraOpacity = new Color(855638016L);
}

public static partial class ShadowsLibrary
{
    internal static Color _kKeyPenumbraOpacity = new Color(603979776L);
}

public static partial class ShadowsLibrary
{
    internal static Color _kAmbientShadowOpacity = new Color(520093696L);
}

public static partial class ShadowsLibrary
{
    internal static DartMap<long, List<BoxShadow>> _elevationToShadow = new DartMap<
        long,
        List<BoxShadow>
    >
    {
        [0L] = new List<BoxShadow>(),
        [1L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 2.0),
                blurRadius: 1.0,
                spreadRadius: -1.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 1.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 3.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [2L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 1.0,
                spreadRadius: -2.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 2.0),
                blurRadius: 2.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 5.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [3L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 3.0,
                spreadRadius: -2.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 4.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 8.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [4L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 2.0),
                blurRadius: 4.0,
                spreadRadius: -1.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 4.0),
                blurRadius: 5.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 10.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [6L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 5.0,
                spreadRadius: -1.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 6.0),
                blurRadius: 10.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 1.0),
                blurRadius: 18.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [8L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 5.0),
                blurRadius: 5.0,
                spreadRadius: -3.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 8.0),
                blurRadius: 10.0,
                spreadRadius: 1.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 14.0,
                spreadRadius: 2.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [9L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 5.0),
                blurRadius: 6.0,
                spreadRadius: -3.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 9.0),
                blurRadius: 12.0,
                spreadRadius: 1.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 3.0),
                blurRadius: 16.0,
                spreadRadius: 2.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [12L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 7.0),
                blurRadius: 8.0,
                spreadRadius: -4.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 12.0),
                blurRadius: 17.0,
                spreadRadius: 2.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 5.0),
                blurRadius: 22.0,
                spreadRadius: 4.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [16L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 8.0),
                blurRadius: 10.0,
                spreadRadius: -5.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 16.0),
                blurRadius: 24.0,
                spreadRadius: 2.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 6.0),
                blurRadius: 30.0,
                spreadRadius: 5.0,
                color: _kAmbientShadowOpacity
            ),
        },
        [24L] = new List<BoxShadow>
        {
            new BoxShadow(
                offset: new Offset(0.0, 11.0),
                blurRadius: 15.0,
                spreadRadius: -7.0,
                color: _kKeyUmbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 24.0),
                blurRadius: 38.0,
                spreadRadius: 3.0,
                color: _kKeyPenumbraOpacity
            ),
            new BoxShadow(
                offset: new Offset(0.0, 9.0),
                blurRadius: 46.0,
                spreadRadius: 8.0,
                color: _kAmbientShadowOpacity
            ),
        },
    };
}
