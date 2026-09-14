#version 440
layout(location = 0) in vec2 qt_TexCoord0;
layout(location = 0) out vec4 fragColor;
layout(std140, binding = 0) uniform buf {
    mat4 qt_Matrix;
    float qt_Opacity;
    vec2 stepSize;
    float sigma;
} ubuf;
layout(binding = 1) uniform sampler2D source;

void main() {
    vec4 sum = texture(source, qt_TexCoord0);
    float weight = 1.0;
    float variance = max(ubuf.sigma * ubuf.sigma, 0.0001);
    int radius = min(96, int(ceil(3.0 * ubuf.sigma)));
    for (int i = 1; i <= radius; ++i) {
        float w = exp(-float(i * i) / (2.0 * variance));
        vec2 offset = ubuf.stepSize * float(i);
        sum += w * (texture(source, qt_TexCoord0 + offset) + texture(source, qt_TexCoord0 - offset));
        weight += 2.0 * w;
    }
    fragColor = sum * (ubuf.qt_Opacity / weight);
}
