# Native texture test clip

`texture-pattern.mp4` is a generated 320×180, 30 fps, three-second H.264 baseline
SDR/YUV420P test pattern, with no audio or third-party footage.

Generation (FFmpeg provided locally by imageio-ffmpeg):

```text
ffmpeg -y -f lavfi -i testsrc2=size=320x180:rate=30 -t 3 -c:v libx264 -profile:v baseline -pix_fmt yuv420p -movflags +faststart texture-pattern.mp4
```

Only the Android opt-in native texture fixture packages and plays this asset.
