# Doroti DisplayList wire format v1

This file is the cross-language contract implemented by `DisplayListEncoder` and
`DisplayListDecoder`. All integer and IEEE-754 fields are little-endian. There
is no implicit alignment or padding; only the reserved bytes listed below are
present. A decoder must reject unknown values rather than skip them.

## Envelope

The transferable buffer is:

```text
fixed header (112 bytes)
resource table (resourceTableByteLength bytes)
UTF-8 string table (stringTableByteLength bytes)
command table (commandByteLength bytes)
```

| Offset | Type | Field |
|---:|---|---|
| 0 | `u32` | magic `0x54534c44` (`DLST` bytes) |
| 4 | `u16` | schema version `1` |
| 6 | `u16` | header size `112` |
| 8 | `u32` | exact total byte length |
| 12 | `u32` | flags: bit 0 checksum present, bit 1 diagnostic capture |
| 16 | `u64` | view id |
| 24 | `u64` | scene sequence |
| 32 | `u64` | build token |
| 40 | `u64` | resize epoch |
| 48 | `u64` | surface generation |
| 56 | `u64` | context generation |
| 64 | `f32` | logical width |
| 68 | `f32` | logical height |
| 72 | `u32` | physical width |
| 76 | `u32` | physical height |
| 80 | `f32` | device pixel ratio |
| 84 | `u32` | command count |
| 88 | `u32` | resource count |
| 92 | `u32` | string-table byte length |
| 96 | `u32` | command-table byte length |
| 100 | `u32` | resource-table byte length (`resource count * 32`) |
| 104 | `u32` | checksum, or zero when bit 0 is absent |
| 108 | `u32` | reserved zero |

All six identities/generations are nonzero. Geometry and DPR are finite and
positive. Canonical `f32` values are finite and encode zero as positive zero;
negative zero is rejected.

The checksum is CRC-32/ISO-HDLC: reflected polynomial `0xedb88320`, initial and
final XOR `0xffffffff`, over the exact buffer while bytes `[104,108)` are read
as zero.

Limits are 64 MiB total, 1,000,000 commands, 65,536 resources, 16 MiB of string
table, 1,000,000 elements in any variable collection, and 32 recursive tagged
value levels.

## Resources and strings

Each fixed 32-byte resource entry is:

```text
kind:u16 flags:u16 version:u32 id:u64 fingerprintLow:u64 fingerprintHigh:u64
```

Kinds are `Font=1`, `Image=2`, `RuntimeEffect=3`, and `RetainedScene=4`.
Resource flag bit 0 means recoverable; all other bits are invalid. IDs and
versions are nonzero. Entries are unique and strictly sorted by
`(kind, id, version)`. `viewId` from the header completes the resource key.

Every resource reference inside a command is exactly 16 bytes:

```text
kind:u16 reserved:u16=0 version:u32 id:u64
```

It must match an entry in this scene's resource table and any required kind at
the use site.

The string table is a sequence of `byteLength:u32` followed by that many strict
UTF-8 bytes. Entries are unique and strictly sorted by raw UTF-8 bytes. A
string reference is its zero-based `u32` table index. Optional strings use
`0xffffffff` for null.

## Commands

Each command is `opcode:u16 flags:u16=0 payloadLength:u32 payload:bytes`.
The payload must be consumed exactly.

| Opcode | Payload |
|---:|---|
| 1 `Save` | empty |
| 2 `Restore` | empty |
| 3 `SaveLayer` | `OptionalRect OptionalPaint` |
| 4 `Transform` | `Matrix` |
| 5 `ClipRect` | `Rect clipOp:u8 antiAlias:bool reserved:u16=0` |
| 6 `ClipRoundedRect` | `RoundedRect clipOp:u8 antiAlias:bool reserved:u16=0` |
| 7 `ClipPath` | `Path clipOp:u8 antiAlias:bool reserved:u16=0` |
| 16 `DrawColor` | `color:u32 blendMode:u8` |
| 17 `DrawPaint` | `Paint` |
| 18 `DrawLine` | `Point Point Paint` |
| 19 `DrawPoints` | `pointMode:u8 count:u32 Point[count] Paint` |
| 20 `DrawRect` | `Rect Paint` |
| 21 `DrawRoundedRect` | `RoundedRect Paint` |
| 22 `DrawDoubleRoundedRect` | `RoundedRect RoundedRect Paint` |
| 23 `DrawCircle` | `Point radius:f32 Paint` |
| 24 `DrawOval` | `Rect Paint` |
| 25 `DrawArc` | `Rect startAngle:f32 sweepAngle:f32 useCenter:bool Paint` |
| 26 `DrawPath` | `Path Paint` |
| 27 `DrawShadow` | `Path color:u32 elevation:f32 transparentOccluder:bool` |
| 28 `DrawImage` | `ImageResourceRef Point sampling:u8 Paint` |
| 29 `DrawImageRect` | `ImageResourceRef Rect(source) Rect(destination) sampling:u8 Paint` |
| 30 `DrawNinePatch` | `ImageResourceRef Rect(center) Rect(destination) sampling:u8 Paint` |
| 31 `DrawParagraph` | `Paragraph Point` |
| 48 `PushOpacity` | `opacity:f32 Point(offset)` |
| 49 `PushColorFilter` | `RequiredColorFilter Point(offset)` |
| 50 `PushImageFilter` | `RequiredImageFilter Point(offset) OptionalRect(bounds)` |
| 51 `PushBackdropFilter` | `RequiredImageFilter blendMode:u8 backdropId:u64 Point(offset)` |
| 52 `PushShaderMask` | `RequiredShader Rect(mask) blendMode:u8` |
| 53 `DrawRetainedScene` | `RetainedSceneResourceRef Point(offset) cacheHint:u8` |

Cache-hint bit 0 is `IsComplex` and bit 1 is `WillChange`.

## Value layouts

`bool` is one byte and must be `0` or `1`.

```text
Point       = x:f32 y:f32
Rect        = left:f32 top:f32 right:f32 bottom:f32
RoundedRect = Rect topLeftX:f32 topLeftY:f32 topRightX:f32 topRightY:f32
              bottomRightX:f32 bottomRightY:f32 bottomLeftX:f32 bottomLeftY:f32
Matrix      = 16 * f32, in producer Matrix4 order
OptionalRect   = present:bool [Rect]
OptionalMatrix = present:bool [Matrix]
OptionalPaint  = present:bool [Paint]
```

Enum values are:

- blend mode `0..28`: Clear, Source, Destination, SourceOver,
  DestinationOver, SourceIn, DestinationIn, SourceOut, DestinationOut,
  SourceAtop, DestinationAtop, Xor, Plus, Modulate, Screen, Overlay, Darken,
  Lighten, ColorDodge, ColorBurn, HardLight, SoftLight, Difference, Exclusion,
  Multiply, Hue, Saturation, Color, Luminosity;
- paint style: Fill `0`, Stroke `1`;
- stroke cap: Butt `0`, Round `1`, Square `2`;
- stroke join: Miter `0`, Round `1`, Bevel `2`;
- sampling: None `0`, Low `1`, Medium `2`, High `3`;
- tile mode: Clamp `0`, Repeat `1`, Mirror `2`, Decal `3`;
- clip operation: Difference `0`, Intersect `1`;
- point mode: Points `0`, Lines `1`, Polygon `2`;
- blur style: Normal `0`, Solid `1`, Outer `2`, Inner `3`;
- path fill: NonZero `0`, EvenOdd `1`;
- font slant: Normal `0`, Italic `1`;
- text direction: LeftToRight `0`, RightToLeft `1`;
- text align: Start `0`, End `1`, Left `2`, Right `3`, Center `4`, Justify `5`.

### Path

```text
fillType:u8 reserved:u8=0 reserved:u16=0
verbCount:u32 valueCount:u32
verbs:u8[verbCount]
values:f32[valueCount]
```

Verb tags and required numeric counts are MoveTo `0/2`, LineTo `1/2`,
RelativeMoveTo `2/2`, RelativeLineTo `3/2`, QuadraticTo `4/4`, ConicTo `5/5`,
CubicTo `6/6`, AddRect `7/4`, AddOval `8/4`, AddArc `9/6`, AddRoundedRect
`10/12`, AddSuperellipse `11/12`, ArcToPoint `12/7`, ArcTo `13/7`, and Close
`14/0`. The sum of the verb requirements must equal `valueCount`.

### Paint

```text
color:u32
style:u8 strokeCap:u8 strokeJoin:u8 antiAlias:bool
blendMode:u8 sampling:u8 invertColors:bool reserved:u8=0
strokeWidth:f32 strokeMiterLimit:f32
Shader ColorFilter MaskFilter ImageFilter
```

Stroke values are nonnegative.

### Shader

Every shader starts with `tag:u8`:

- `0`: null; only legal at optional sites.
- `1` linear gradient: `Point(start) Point(end) tileMode:u8 Gradient OptionalMatrix`.
- `2` radial gradient: `Point(center) radius:f32 tileMode:u8 hasFocal:bool
  [Point(focal)] focalRadius:f32 Gradient OptionalMatrix`.
- `3` sweep gradient: `Point(center) startAngle:f32 endAngle:f32 tileMode:u8
  Gradient OptionalMatrix`.
- `4` image shader: `ImageResourceRef tileX:u8 tileY:u8 sampling:u8
  reserved:u8=0 Matrix`.
- `5` runtime effect: `RuntimeEffectResourceRef uniformByteCount:u32
  uniforms:u8[uniformByteCount] childCount:u32 ResourceRef[childCount]`.

`Gradient` is `count:u32` followed by `count` pairs of
`color:u32 stop:f32`. Count is at least two and stops are nondecreasing.

### Filters

Every color filter starts with `tag:u8`:

- `0`: null; optional sites only.
- `1`: `color:u32 blendMode:u8`.
- `2`: `matrix:f32[20]`.
- `3`: linear-to-sRGB gamma, no payload.
- `4`: sRGB-to-linear gamma, no payload.

`MaskFilter` is `present:bool`, followed when present by
`blurStyle:u8 sigma:f32`.

Every image filter starts with `tag:u8`:

- `0`: null; optional sites only.
- `1` blur: `sigmaX:f32 sigmaY:f32 tileMode:u8 OptionalRect`.
- `2` color: `RequiredColorFilter`.
- `3` matrix: `Matrix sampling:u8`.
- `4` runtime effect: `RequiredShader(tag 5) sampling:u8`.
- `5` compose: `RequiredImageFilter(outer) RequiredImageFilter(inner)`.
- `6` drop shadow: `deltaX:f32 deltaY:f32 sigmaX:f32 sigmaY:f32
  color:u32 shadowOnly:bool`.

Sigma values are nonnegative. Required tagged values cannot use tag zero.

### Paragraph

```text
textStringId:u32
FontResourceRef
fontFamilyStringId:u32
localeStringId:u32
ellipsisStringIdOrFFFFFFFF:u32
fontSize:f32
heightMultiplier:f32
color:u32
fontWeight:i32
fontSlant:u8 direction:u8 align:u8 reserved:u8=0
maxLines:u32
layoutWidth:f32 measuredWidth:f32 measuredHeight:f32
metricsHash:u64
fallbackCount:u32
FontResourceRef[fallbackCount]
```

Font size and height multiplier are positive; widths and height are
nonnegative; font weight is `1..1000`.

## Cross-language golden

`Doroti/validation/display-list-contract/golden/display-list-v1-full.json`
contains a base64 buffer covering every opcode and every tagged-value family,
plus its byte length and SHA-256. Regenerate it from the validation project
with `--emit-golden-json`; golden updates require an intentional schema review.
