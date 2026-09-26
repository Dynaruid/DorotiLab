#version 450
layout(set = 0, binding = 0) uniform texture2D inputImage;
layout(set = 0, binding = 1) uniform sampler inputSampler;
layout(location = 0) in vec2 uv;
layout(location = 0) out vec4 color;
void main() { color = texture(sampler2D(inputImage, inputSampler), uv).bgra; }
