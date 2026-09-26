struct Frame {
    input_size: vec2<u32>, output_size: vec2<u32>, logical_size: vec2<f32>, time: f32, delta_time: f32
}
@group(0) @binding(0) var input_image: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;
@group(0) @binding(2) var<uniform> frame: Frame;
@fragment
fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> {
    let gain = vec4<f32>(frame.time, frame.delta_time * 4.,
        frame.logical_size.y * 2. / f32(frame.output_size.y), frame.logical_size.x * 2. / f32(frame.input_size.x));
    return textureSample(input_image, input_sampler, uv).bgra * gain;
}
