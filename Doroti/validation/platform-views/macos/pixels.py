"""Small dependency-free decoder for the 8-bit RGB/RGBA PNGs produced by screencapture."""
import struct
import zlib


def read_png(path, max_row=None):
    data = path.read_bytes()
    assert data[:8] == b"\x89PNG\r\n\x1a\n"
    offset, compressed = 8, bytearray()
    while offset < len(data):
        length = struct.unpack_from(">I", data, offset)[0]
        kind = data[offset + 4:offset + 8]
        chunk = data[offset + 8:offset + 8 + length]
        if kind == b"IHDR":
            width, height, depth, color, _, _, interlace = struct.unpack(">IIBBBBB", chunk)
            assert depth == 8 and color in (2, 6) and interlace == 0, (depth, color, interlace)
            bpp = 3 if color == 2 else 4
        elif kind == b"IDAT": compressed.extend(chunk)
        offset += length + 12
    raw = zlib.decompress(compressed)
    stride = width * bpp
    rows, previous = [], bytearray(stride)
    for y in range(height if max_row is None else min(height, max_row + 1)):
        start = y * (stride + 1)
        mode, row = raw[start], bytearray(raw[start + 1:start + 1 + stride])
        for x in range(stride):
            a = row[x - bpp] if x >= bpp else 0
            b = previous[x]
            c = previous[x - bpp] if x >= bpp else 0
            if mode == 1: prediction = a
            elif mode == 2: prediction = b
            elif mode == 3: prediction = (a + b) // 2
            elif mode == 4:
                p = a + b - c
                pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
                prediction = a if pa <= pb and pa <= pc else b if pb <= pc else c
            else:
                assert mode == 0
                prediction = 0
            row[x] = (row[x] + prediction) & 255
        rows.append(row)
        previous = row
    return width, height, lambda x, y: tuple(rows[y][x * bpp:x * bpp + 3])


def validate(stages, artifacts):
    images = {stage["stage"]: read_png(artifacts / stage["pixelScreenshot"],
        round((stage["overlayOrigin"][1] + stage["sceneOrigin"][1] + 160) * stage["backingScale"])) for stage in stages}
    metadata = {stage["stage"]: stage for stage in stages}

    def pixel(stage, x, y):
        data = metadata[stage]
        width, height, sample = images[stage]
        scale = data["backingScale"]
        px = round((data["overlayOrigin"][0] + data["sceneOrigin"][0] + x) * scale)
        py = round((data["overlayOrigin"][1] + data["sceneOrigin"][1] + y) * scale)
        assert 0 <= px < width and 0 <= py < height, (stage, px, py, width, height)
        return sample(px, py)

    def close(actual, expected, label, tolerance=12):
        assert max(abs(a - b) for a, b in zip(actual, expected)) <= tolerance, (label, actual, expected)

    baseline = pixel(0, 300, 160)
    close(pixel(1, 300, 160), (255, 51, 0), "partial opaque cover")
    close(pixel(2, 300, 160), (255, 51, 0), "full editor cover")
    close(pixel(2, 100, 70), (255, 51, 0), "full button cover")
    close(pixel(3, 300, 160), baseline, "uncover restores native pixels")
    close(pixel(4, 300, 160), baseline, "native above opaque foreground")
    close(pixel(5, 200, 70), (0, 170, 85), "middle raster above native A")
    close(pixel(5, 200, 110), pixel(0, 200, 110), "native B above middle raster")
    alpha = tuple(round(top * .6 + bottom * .4) for top, bottom in zip((255, 51, 0), baseline))
    close(pixel(6, 300, 160), alpha, "alpha over live native", tolerance=18)
    close(pixel(7, 300, 160), pixel(6, 300, 160), "shield off preserves visual composition")
    close(pixel(8, 300, 160), baseline, "native above reordered shield")
    return {"passed": True, "nativeBaselineRgb": baseline, "alphaRgb": pixel(6, 300, 160)}
