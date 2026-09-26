import { effectGl, effectGpu, registerEffectAllocation, accountEffectBytes } from "./doroti.web.texture-worker.js";

interface Pipeline { program: WebGLProgram; vao: WebGLVertexArrayObject; blocks: { index: number; slot: number; size: number; copies: { source: number; destination: number }[] }[]; }
interface Effect { gl: WebGL2RenderingContext; input: WebGLTexture; output: WebGLTexture; framebuffer: WebGLFramebuffer; width: number; height: number; buffers: WebGLBuffer[]; }
const effects = new Map<number, Effect>();
const gpuEffects = new Map<number, number>();
const pipelines = new WeakMap<WebGL2RenderingContext, Map<string, Pipeline>>();
export function releaseEffectPrograms(gl: WebGL2RenderingContext): void {
  const cache = pipelines.get(gl); if (!cache) return;
  for (const pipeline of cache.values()) { gl.deleteProgram(pipeline.program); gl.deleteVertexArray(pipeline.vao); }
  cache.clear(); pipelines.delete(gl);
}

function state<T>(gl: WebGL2RenderingContext, action: () => T): T {
  const program = gl.getParameter(gl.CURRENT_PROGRAM) as WebGLProgram | null;
  const draw = gl.getParameter(gl.DRAW_FRAMEBUFFER_BINDING) as WebGLFramebuffer | null;
  const read = gl.getParameter(gl.READ_FRAMEBUFFER_BINDING) as WebGLFramebuffer | null;
  const vao = gl.getParameter(gl.VERTEX_ARRAY_BINDING) as WebGLVertexArrayObject | null;
  const active = gl.getParameter(gl.ACTIVE_TEXTURE) as number;
  const viewport = gl.getParameter(gl.VIEWPORT) as Int32Array;
  const scissor = gl.getParameter(gl.SCISSOR_BOX) as Int32Array;
  const mask = gl.getParameter(gl.COLOR_WRITEMASK) as boolean[];
  const uniform = gl.getParameter(gl.UNIFORM_BUFFER_BINDING) as WebGLBuffer | null;
  const bindings = [0, 1].map(index => ({
    buffer: gl.getIndexedParameter(gl.UNIFORM_BUFFER_BINDING, index) as WebGLBuffer | null,
    start: gl.getIndexedParameter(gl.UNIFORM_BUFFER_START, index) as number,
    size: gl.getIndexedParameter(gl.UNIFORM_BUFFER_SIZE, index) as number,
  }));
  const caps = [gl.BLEND, gl.DEPTH_TEST, gl.STENCIL_TEST, gl.SCISSOR_TEST, gl.CULL_FACE,
    gl.RASTERIZER_DISCARD, gl.SAMPLE_ALPHA_TO_COVERAGE, gl.SAMPLE_COVERAGE];
  const enabled = caps.map(cap => gl.isEnabled(cap));
  gl.activeTexture(gl.TEXTURE0);
  const texture = gl.getParameter(gl.TEXTURE_BINDING_2D) as WebGLTexture | null;
  const sampler = gl.getParameter(gl.SAMPLER_BINDING) as WebGLSampler | null;
  try { return action(); }
  finally {
    gl.useProgram(program); gl.bindVertexArray(vao);
    gl.bindFramebuffer(gl.READ_FRAMEBUFFER, read); gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, draw);
    gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D, texture); gl.bindSampler(0, sampler); gl.activeTexture(active);
    bindings.forEach((binding, index) => {
      if (binding.buffer && binding.size) gl.bindBufferRange(gl.UNIFORM_BUFFER, index, binding.buffer, binding.start, binding.size);
      else gl.bindBufferBase(gl.UNIFORM_BUFFER, index, binding.buffer);
    });
    gl.bindBuffer(gl.UNIFORM_BUFFER, uniform);
    gl.viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
    gl.scissor(scissor[0], scissor[1], scissor[2], scissor[3]);
    gl.colorMask(mask[0], mask[1], mask[2], mask[3]);
    caps.forEach((cap, index) => enabled[index] ? gl.enable(cap) : gl.disable(cap));
  }
}

export function allocateEffect(width: number, height: number): { token: number; input: number; output: number } {
  const gpu = effectGpu();
  if (gpu) {
    const allocation = gpu.allocateGpuEffect(width, height);
    let token = 0;
    try {
      token = registerEffectAllocation(width * height * 8, allocation.output, () => { allocation.destroy(); gpuEffects.delete(token); });
      gpuEffects.set(token, allocation.input);
      return { token, input: allocation.input, output: allocation.output };
    } catch (error) { allocation.destroy(); throw error; }
  }
  const table = effectGl(); const gl = table.currentContext.GLctx;
  if (gl.isContextLost() || !Number.isSafeInteger(width) || !Number.isSafeInteger(height) || width < 1 || height < 1 ||
      width > gl.getParameter(gl.MAX_TEXTURE_SIZE) || height > gl.getParameter(gl.MAX_TEXTURE_SIZE) || width * height * 8 > 64 * 1024 * 1024)
    throw new Error("GPU effect texture dimensions exceed context limits.");
  return state(gl, () => {
    const textures: WebGLTexture[] = []; const handles: number[] = [];
    let framebuffer: WebGLFramebuffer | null = null;
    const buffers: WebGLBuffer[] = [];
    let token = 0;
    const destroy = (): void => {
      for (const handle of handles) table.textures[handle] = null;
      textures.forEach(texture => gl.deleteTexture(texture));
      buffers.forEach(buffer => gl.deleteBuffer(buffer));
      gl.deleteFramebuffer(framebuffer); effects.delete(token);
    };
    try {
      for (let index = 0; index < 2; index++) {
        const texture = gl.createTexture(); if (!texture) throw new Error("Effect texture allocation failed.");
        textures.push(texture); gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texStorage2D(gl.TEXTURE_2D, 1, gl.RGBA8, width, height);
        const handle = table.getNewId(table.textures); handles.push(handle);
        table.textures[handle] = texture; (texture as WebGLTexture & { name: number }).name = handle;
      }
      framebuffer = gl.createFramebuffer(); if (!framebuffer) throw new Error("Effect FBO allocation failed.");
      gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, framebuffer);
      gl.framebufferTexture2D(gl.DRAW_FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, textures[1], 0);
      if (gl.checkFramebufferStatus(gl.DRAW_FRAMEBUFFER) !== gl.FRAMEBUFFER_COMPLETE) throw new Error("Effect FBO is incomplete.");
      token = registerEffectAllocation(width * height * 8, handles[1], destroy);
      effects.set(token, { gl, input: textures[0], output: textures[1], framebuffer, width, height, buffers });
      return { token, input: handles[0], output: handles[1] };
    } catch (error) { destroy(); throw error; }
  });
}

function shader(gl: WebGL2RenderingContext, type: number, source: string): WebGLShader {
  const shader = gl.createShader(type); if (!shader) throw new Error("Effect shader allocation failed.");
  gl.shaderSource(shader, source); gl.compileShader(shader);
  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    const log = gl.getShaderInfoLog(shader); gl.deleteShader(shader); throw new Error(`WGSL variant compilation: ${log}`);
  }
  return shader;
}

function pipeline(gl: WebGL2RenderingContext, key: string, vertex: string, fragment: string, metadata: string): Pipeline {
  let cache = pipelines.get(gl); if (!cache) { cache = new Map(); pipelines.set(gl, cache); }
  const cached = cache.get(key); if (cached) return cached;
  const vs = shader(gl, gl.VERTEX_SHADER, vertex);
  let fs: WebGLShader | null = null; let program: WebGLProgram | null = null; let vao: WebGLVertexArrayObject | null = null;
  try {
    fs = shader(gl, gl.FRAGMENT_SHADER, fragment); program = gl.createProgram(); vao = gl.createVertexArray();
    if (!program || !vao) throw new Error("Effect pipeline allocation failed.");
    gl.attachShader(program, vs); gl.attachShader(program, fs); gl.linkProgram(program);
    if (!gl.getProgramParameter(program, gl.LINK_STATUS)) throw new Error(`WGSL variant link: ${gl.getProgramInfoLog(program)}`);
    gl.useProgram(program);
    const bindings = JSON.parse(metadata) as { samplers: { name: string }[]; uniforms: { name: string; binding: { group: number; binding: number }; size: number; fields: { name: string; offset: number }[] }[] };
    for (const sampler of bindings.samplers) gl.uniform1i(gl.getUniformLocation(program, sampler.name), 0);
    const blocks: Pipeline["blocks"] = [];
    const count = gl.getProgramParameter(program, gl.ACTIVE_UNIFORM_BLOCKS) as number;
    for (let index = 0; index < count; index++) {
      const indices = gl.getActiveUniformBlockParameter(program, index, gl.UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES) as number[];
      const names = Array.from(indices, uniform => gl.getActiveUniform(program!, uniform)!.name);
      const blockName = gl.getActiveUniformBlockName(program, index);
      const mapping = bindings.uniforms.find(uniform => uniform.name === blockName);
      if (!mapping) throw new Error(`WGSL uniform block has no reflection mapping: ${names.join(',')}; reflected=${bindings.uniforms.map(u => u.name).join(',')}`);
      const size = gl.getActiveUniformBlockParameter(program, index, gl.UNIFORM_BLOCK_DATA_SIZE) as number;
      if (size > gl.getParameter(gl.MAX_UNIFORM_BLOCK_SIZE)) throw new Error("GLSL uniform block exceeds context limits.");
      const slot = mapping.binding.group === 1 ? 1 : 0;
      const offsets = gl.getActiveUniforms(program, Array.from(indices), gl.UNIFORM_OFFSET) as number[];
      const arrays = gl.getActiveUniforms(program, Array.from(indices), gl.UNIFORM_ARRAY_STRIDE) as number[];
      const matrices = gl.getActiveUniforms(program, Array.from(indices), gl.UNIFORM_MATRIX_STRIDE) as number[];
      const copies: { source: number; destination: number }[] = [];
      Array.from(indices).forEach((uniformIndex, member) => {
        const uniform = gl.getActiveUniform(program!, uniformIndex)!;
        const shape = uniformShape(gl, uniform.type);
        const qualified = uniform.name.startsWith(mapping.name + '.') ? uniform.name.slice(mapping.name.length + 1) : uniform.name;
        const path = qualified.slice(qualified.indexOf('.') + 1);
        for (let element = 0; element < uniform.size; element++) {
          const memberPath = uniform.size > 1 ? path.replace(/\[0\](?!.*\[0\])/, `[${element}]`) : path;
          const name = 'p_' + memberPath.replace(/\[(\d+)\]/g, '_$1').replace(/\./g, '_');
          for (let column = 0; column < shape.columns; column++) for (let row = 0; row < shape.rows; row++) {
            const fieldName = name + (shape.columns > 1 ? `_${column}_${row}` : shape.rows > 1 ? `_${row}` : '');
            const field = mapping.fields.find(field => field.name === fieldName);
            if (!field) throw new Error(`WGSL field mapping unavailable for ${uniform.name}: ${fieldName}`);
            const destination = offsets[member] + element * arrays[member] + (shape.columns > 1 ? column * matrices[member] : 0) + row * 4;
            if (field.offset + 4 > mapping.size || destination + 4 > size) throw new Error("Invalid reflected uniform byte range.");
            copies.push({ source: field.offset, destination });
          }
        }
      });
      gl.uniformBlockBinding(program, index, slot); blocks.push({ index, slot, size, copies });
    }
    const result = { program, vao, blocks };
    if (cache.size >= 64) {
      const oldest = cache.entries().next().value!;
      gl.deleteProgram(oldest[1].program); gl.deleteVertexArray(oldest[1].vao); cache.delete(oldest[0]);
    }
    cache.set(key, result); program = null; vao = null;
    return result;
  } finally { gl.deleteShader(vs); gl.deleteShader(fs); gl.deleteProgram(program); gl.deleteVertexArray(vao); }
}

function uniformShape(gl: WebGL2RenderingContext, type: number): { columns: number; rows: number } {
  const shapes = new Map<number, [number, number]>([
    [gl.FLOAT, [1, 1]], [gl.INT, [1, 1]], [gl.UNSIGNED_INT, [1, 1]],
    [gl.FLOAT_VEC2, [1, 2]], [gl.INT_VEC2, [1, 2]], [gl.UNSIGNED_INT_VEC2, [1, 2]],
    [gl.FLOAT_VEC3, [1, 3]], [gl.INT_VEC3, [1, 3]], [gl.UNSIGNED_INT_VEC3, [1, 3]],
    [gl.FLOAT_VEC4, [1, 4]], [gl.INT_VEC4, [1, 4]], [gl.UNSIGNED_INT_VEC4, [1, 4]],
    [gl.FLOAT_MAT2, [2, 2]], [gl.FLOAT_MAT3, [3, 3]], [gl.FLOAT_MAT4, [4, 4]],
    [gl.FLOAT_MAT2x3, [2, 3]], [gl.FLOAT_MAT2x4, [2, 4]], [gl.FLOAT_MAT3x2, [3, 2]],
    [gl.FLOAT_MAT3x4, [3, 4]], [gl.FLOAT_MAT4x2, [4, 2]], [gl.FLOAT_MAT4x3, [4, 3]],
  ]);
  const shape = shapes.get(type); if (!shape) throw new Error(`Unsupported GLSL uniform type ${type}.`);
  return { columns: shape[0], rows: shape[1] };
}

export function executeEffect(token: number, key: string, vertex: string, fragment: string, entry: string, metadata: string, encodedParameters: string, time = 0, deltaTime = 0, logicalWidth = 0, logicalHeight = 0): void {
  const parameters = Uint8Array.from(atob(encodedParameters), c => c.charCodeAt(0));
  const gpu = effectGpu();
  if (gpu) {
    const handle = gpuEffects.get(token); if (!handle) throw new Error("Stale WebGPU effect lease.");
    accountEffectBytes(token, 32 + Math.max(16, Math.ceil(parameters.length / 16) * 16));
    gpu.executeGpuEffect(handle, key, vertex, fragment, entry, parameters, time, deltaTime, logicalWidth, logicalHeight);
    return;
  }
  const effect = effects.get(token); if (!effect) throw new Error("Stale GPU effect texture lease.");
  const gl = effectGl().currentContext.GLctx;
  if (effect.gl !== gl || gl.isContextLost()) throw new Error("GPU effect context generation changed.");
  state(gl, () => {
    const pass = pipeline(gl, key, vertex, fragment, metadata);
    gl.useProgram(pass.program); gl.bindVertexArray(pass.vao);
    gl.bindFramebuffer(gl.DRAW_FRAMEBUFFER, effect.framebuffer);
    gl.viewport(0, 0, effect.width, effect.height); gl.colorMask(true, true, true, true);
    for (const cap of [gl.BLEND, gl.DEPTH_TEST, gl.STENCIL_TEST, gl.SCISSOR_TEST, gl.CULL_FACE,
      gl.RASTERIZER_DISCARD, gl.SAMPLE_ALPHA_TO_COVERAGE, gl.SAMPLE_COVERAGE]) gl.disable(cap);
    gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D, effect.input); gl.bindSampler(0, null);
    const frame = new ArrayBuffer(32); const data = new DataView(frame);
    data.setUint32(0, effect.width, true); data.setUint32(4, effect.height, true);
    data.setUint32(8, effect.width, true); data.setUint32(12, effect.height, true);
    data.setFloat32(16, logicalWidth || effect.width, true); data.setFloat32(20, logicalHeight || effect.height, true);
    data.setFloat32(24, time, true); data.setFloat32(28, deltaTime, true);
    for (const block of pass.blocks) {
      const bytes = block.slot === 0 ? new Uint8Array(frame) : parameters;
      const padded = new Uint8Array(block.size);
      for (const copy of block.copies) {
        if (copy.source + 4 > bytes.length) throw new Error("GPU effect uniform bytes are incomplete.");
        padded.set(bytes.subarray(copy.source, copy.source + 4), copy.destination);
      }
      accountEffectBytes(token, block.size);
      const buffer = gl.createBuffer(); if (!buffer) throw new Error("Effect uniform buffer allocation failed.");
      effect.buffers.push(buffer); gl.bindBuffer(gl.UNIFORM_BUFFER, buffer);
      gl.bufferData(gl.UNIFORM_BUFFER, padded, gl.STREAM_DRAW); gl.bindBufferBase(gl.UNIFORM_BUFFER, block.slot, buffer);
    }
    gl.drawArrays(gl.TRIANGLES, 0, 3);
    const error = gl.getError(); if (error !== gl.NO_ERROR) throw new Error(`WebGL GPU effect error ${error}.`);
  });
}
