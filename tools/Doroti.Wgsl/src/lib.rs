use naga::{AddressSpace, ShaderStage, TypeInner};
use serde::{Deserialize, Serialize};
use sha2::{Digest, Sha256};
use std::{fs, path::Path};

type Result<T> = std::result::Result<T, Box<dyn std::error::Error>>;
const COMPILER: &str = "doroti-wgsl/0.1.0+naga/30.0.1";
const VERTEX: &str = r#"
struct VertexOutput { @builtin(position) position: vec4<f32>, @location(0) uv: vec2<f32> }
@vertex fn main(@builtin(vertex_index) index: u32) -> VertexOutput {
    let uv = vec2<f32>(f32((index << 1u) & 2u), f32(index & 2u));
    return VertexOutput(vec4<f32>(uv * vec2<f32>(2.0, -2.0) + vec2<f32>(-1.0, 1.0), 0.0, 1.0), uv);
}
"#;
const GL_VERTEX: &str = "#version 300 es\nprecision highp float;\nsmooth out vec2 _vs2fs_location0;\nvoid main(){ vec2 uv=vec2(float((gl_VertexID<<1)&2),float(gl_VertexID&2)); _vs2fs_location0=uv; gl_Position=vec4(uv*2.0-1.0,0.0,1.0); }\n";

#[derive(Deserialize)]
#[serde(rename_all = "camelCase", deny_unknown_fields)]
struct Definition {
    schema: u32,
    asset_id: String,
    entry_point: String,
    required_backends: Vec<String>,
}

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
struct Field {
    name: String,
    offset: u32,
    scalar: String,
}

fn fields(
    module: &naga::Module,
    ty: naga::Handle<naga::Type>,
    prefix: &str,
    base: u32,
    layout: &naga::proc::Layouter,
    result: &mut Vec<Field>,
) -> Result<()> {
    if result.len() > 4096 {
        return Err("Uniform exceeds the serializer field limit".into());
    }
    let scalar_name = |scalar: naga::Scalar| -> Result<String> {
        if scalar.width != 4 {
            return Err("Only 32-bit uniform scalars are supported".into());
        }
        Ok(match scalar.kind {
            naga::ScalarKind::Float => "f32",
            naga::ScalarKind::Sint => "i32",
            naga::ScalarKind::Uint => "u32",
            _ => return Err("Unsupported uniform scalar".into()),
        }
        .into())
    };
    match &module.types[ty].inner {
        TypeInner::Scalar(s) => result.push(Field {
            name: prefix.into(),
            offset: base,
            scalar: scalar_name(*s)?,
        }),
        TypeInner::Vector { size, scalar } => {
            for i in 0..*size as u32 {
                result.push(Field {
                    name: format!("{prefix}_{i}"),
                    offset: base + i * 4,
                    scalar: scalar_name(*scalar)?,
                });
            }
        }
        TypeInner::Matrix {
            columns,
            rows,
            scalar,
        } => {
            let stride = layout[ty].size / *columns as u32;
            for column in 0..*columns as u32 {
                for row in 0..*rows as u32 {
                    result.push(Field {
                        name: format!("{prefix}_{column}_{row}"),
                        offset: base + column * stride + row * 4,
                        scalar: scalar_name(*scalar)?,
                    });
                }
            }
        }
        TypeInner::Array {
            base: element,
            size: naga::ArraySize::Constant(count),
            stride,
        } => {
            if count.get() > 4096 {
                return Err("Uniform array exceeds the serializer field limit".into());
            }
            for i in 0..count.get() {
                fields(
                    module,
                    *element,
                    &format!("{prefix}_{i}"),
                    base + i * stride,
                    layout,
                    result,
                )?;
            }
        }
        TypeInner::Struct { members, .. } => {
            for (i, member) in members.iter().enumerate() {
                let name = member.name.clone().unwrap_or(format!("field{i}"));
                fields(
                    module,
                    member.ty,
                    &format!("{prefix}_{name}"),
                    base + member.offset,
                    layout,
                    result,
                )?;
            }
        }
        _ => return Err("Uniform type cannot be serialized by ABI 1".into()),
    }
    Ok(())
}

fn hash(bytes: &[u8]) -> String {
    format!("{:x}", Sha256::digest(bytes))
}

pub fn run(args: Vec<String>) -> Result<()> {
    if args.len() != 4
        || !["compile", "validate", "reflect", "generate"].contains(&args[0].as_str())
    {
        return Err("Usage: doroti-wgsl <compile|validate|reflect|generate> source.wgsl definition.effect.json output-directory".into());
    }
    let source = fs::read_to_string(&args[1])?;
    let definition_bytes = fs::read(&args[2])?;
    let definition: Definition = serde_json::from_slice(&definition_bytes)?;
    if definition.schema != 1
        || definition.asset_id.is_empty()
        || definition.required_backends.is_empty()
    {
        return Err("Definition requires schema 1, assetId and requiredBackends".into());
    }
    if definition.required_backends.iter().any(|p| {
        ![
            "vulkan-fragment",
            "webgpu-fragment",
            "webgl2-fragment",
            "metal-fragment",
        ]
        .contains(&p.as_str())
    }) {
        return Err("Unknown requiredBackends profile".into());
    }
    let module = naga::front::wgsl::parse_str(&source)
        .map_err(|e| e.emit_to_string_with_path(&source, &args[1]))?;
    let info = naga::valid::Validator::new(
        naga::valid::ValidationFlags::all(),
        naga::valid::Capabilities::empty(),
    )
    .validate(&module)
    .map_err(|e| e.emit_to_string_with_path(&source, &args[1]))?;
    let ep = module
        .entry_points
        .iter()
        .find(|ep| ep.name == definition.entry_point)
        .ok_or("Definition entryPoint is absent from the WGSL module")?;
    if ep.stage != ShaderStage::Fragment {
        return Err("ABI 1 currently requires a fragment entry point".into());
    }
    if ep.function.arguments.len() != 1
        || !matches!(
            &ep.function.arguments[0].binding,
            Some(naga::Binding::Location { location: 0, .. })
        )
        || !matches!(
            module.types[ep.function.arguments[0].ty].inner,
            TypeInner::Vector {
                size: naga::VectorSize::Bi,
                scalar: naga::Scalar {
                    kind: naga::ScalarKind::Float,
                    width: 4
                }
            }
        )
    {
        return Err("Fragment ABI requires a single @location(0) vec2<f32> UV argument".into());
    }
    let result = ep
        .function
        .result
        .as_ref()
        .ok_or("Fragment output is missing")?;
    if !matches!(
        &result.binding,
        Some(naga::Binding::Location { location: 0, .. })
    ) || !matches!(
        module.types[result.ty].inner,
        TypeInner::Vector {
            size: naga::VectorSize::Quad,
            scalar: naga::Scalar {
                kind: naga::ScalarKind::Float,
                width: 4
            }
        }
    ) {
        return Err("Fragment ABI requires @location(0) vec4<f32> output".into());
    }
    let mut layout = naga::proc::Layouter::default();
    layout.update(module.to_ctx())?;
    let mut uniform_fields = Vec::new();
    let mut uniform_size = 0;
    let mut resources = Vec::new();
    for (_, global) in module.global_variables.iter() {
        if let Some(binding) = &global.binding {
            let allowed = match (
                binding.group,
                binding.binding,
                &module.types[global.ty].inner,
                global.space,
            ) {
                (
                    0,
                    0,
                    TypeInner::Image {
                        dim: naga::ImageDimension::D2,
                        arrayed: false,
                        class:
                            naga::ImageClass::Sampled {
                                kind: naga::ScalarKind::Float,
                                multi: false,
                            },
                    },
                    AddressSpace::Handle,
                ) => true,
                (0, 1, TypeInner::Sampler { comparison: false }, AddressSpace::Handle) => true,
                (0, 2, TypeInner::Struct { span: 32, members }, AddressSpace::Uniform) => {
                    let expected = [
                        (0, 2, naga::ScalarKind::Uint),
                        (8, 2, naga::ScalarKind::Uint),
                        (16, 2, naga::ScalarKind::Float),
                        (24, 1, naga::ScalarKind::Float),
                        (28, 1, naga::ScalarKind::Float),
                    ];
                    members.len() == expected.len()
                        && members
                            .iter()
                            .zip(expected)
                            .all(|(member, (offset, count, kind))| {
                                member.offset == offset
                                    && match module.types[member.ty].inner {
                                        TypeInner::Scalar(s) => {
                                            count == 1 && s.width == 4 && s.kind == kind
                                        }
                                        TypeInner::Vector { size, scalar } => {
                                            size as u32 == count
                                                && scalar.width == 4
                                                && scalar.kind == kind
                                        }
                                        _ => false,
                                    }
                            })
                }
                (1, 0, TypeInner::Struct { .. }, AddressSpace::Uniform) => {
                    uniform_size = layout[global.ty].size;
                    fields(&module, global.ty, "p", 0, &layout, &mut uniform_fields)?;
                    true
                }
                _ => false,
            };
            if !allowed {
                return Err(format!(
                    "Invalid reserved/resource binding @group({}) @binding({})",
                    binding.group, binding.binding
                )
                .into());
            }
            resources.push(
                serde_json::json!({"group": binding.group, "binding": binding.binding,
                "size": layout[global.ty].size, "name": global.name }),
            );
        }
    }
    let mut names = std::collections::HashSet::new();
    if uniform_fields.len() > 4096 || !uniform_fields.iter().all(|f| names.insert(&f.name)) {
        return Err(
            "Uniform exceeds serializer limits or has ambiguous flattened field names".into(),
        );
    }
    if args[0] == "validate" {
        return Ok(());
    }
    if !definition
        .asset_id
        .chars()
        .enumerate()
        .all(|(i, c)| c.is_ascii_alphabetic() || c == '_' || (i > 0 && c.is_ascii_digit()))
    {
        return Err(
            "assetId must be a C# identifier using ASCII letters, digits and underscore".into(),
        );
    }
    let vertex_module = naga::front::wgsl::parse_str(VERTEX)?;
    let vertex_info = naga::valid::Validator::new(
        naga::valid::ValidationFlags::all(),
        naga::valid::Capabilities::empty(),
    )
    .validate(&vertex_module)?;
    let vertex_spv = naga::back::spv::write_vec(
        &vertex_module,
        &vertex_info,
        &Default::default(),
        Some(&naga::back::spv::PipelineOptions {
            shader_stage: ShaderStage::Vertex,
            entry_point: "main".into(),
        }),
    )?
    .into_iter()
    .flat_map(u32::to_le_bytes)
    .collect::<Vec<_>>();
    let output = Path::new(&args[3]);
    fs::create_dir_all(output)?;
    fs::write(output.join("vertex.spv"), &vertex_spv)?;
    fs::write(output.join("vertex.glsl"), GL_VERTEX)?;
    fs::write(output.join("vertex.wgsl"), VERTEX)?;
    let (metal_vertex, _) = naga::back::msl::write_string(
        &vertex_module,
        &vertex_info,
        &naga::back::msl::Options {
            fake_missing_bindings: false,
            ..Default::default()
        },
        &Default::default(),
    )?;
    fs::write(output.join("vertex.metal"), &metal_vertex)?;
    let mut variants = Vec::new();
    for profile in &definition.required_backends {
        let mut backend_entry = ep.name.clone();
        let (filename, bytes) = match profile.as_str() {
            "vulkan-fragment" => {
                let options = naga::back::spv::Options {
                    lang_version: (1, 3),
                    ..Default::default()
                };
                let pipeline = naga::back::spv::PipelineOptions {
                    shader_stage: ep.stage,
                    entry_point: ep.name.clone(),
                };
                let words = naga::back::spv::write_vec(&module, &info, &options, Some(&pipeline))?;
                (
                    "fragment.spv",
                    words
                        .into_iter()
                        .flat_map(u32::to_le_bytes)
                        .collect::<Vec<_>>(),
                )
            }
            "webgpu-fragment" => ("fragment.wgsl", source.as_bytes().to_vec()),
            "webgl2-fragment" => {
                let mut text = String::new();
                let options = naga::back::glsl::Options {
                    version: naga::back::glsl::Version::Embedded {
                        version: 300,
                        is_webgl: true,
                    },
                    ..Default::default()
                };
                let pipeline = naga::back::glsl::PipelineOptions {
                    shader_stage: ep.stage,
                    entry_point: ep.name.clone(),
                    multiview: None,
                };
                let reflection = naga::back::glsl::Writer::new(
                    &mut text,
                    &module,
                    &info,
                    &options,
                    &pipeline,
                    naga::proc::BoundsCheckPolicies::default(),
                )?
                .write()?;
                let binding = |h: naga::Handle<naga::GlobalVariable>| {
                    module.global_variables[h]
                        .binding
                        .as_ref()
                        .map(|b| serde_json::json!({ "group": b.group, "binding": b.binding }))
                };
                backend_entry = "main".into();
                let mut mapping: Vec<_> = reflection
                    .texture_mapping
                    .iter()
                    .map(|(name, mapping)| {
                        serde_json::json!({ "name": name, "texture": binding(mapping.texture),
                        "sampler": mapping.sampler.and_then(binding) })
                    })
                    .collect();
                let mut uniforms: Vec<_> = reflection.uniforms.iter().map(|(handle, name)| -> Result<_> {
                    let mut members = Vec::new();
                    fields(&module, module.global_variables[*handle].ty, "p", 0, &layout, &mut members)?;
                    Ok(serde_json::json!({ "name": name, "binding": binding(*handle), "size": layout[module.global_variables[*handle].ty].size, "fields": members }))
                }).collect::<Result<_>>()?;
                mapping.sort_by_key(|value| value["name"].as_str().unwrap_or_default().to_string());
                uniforms
                    .sort_by_key(|value| value["name"].as_str().unwrap_or_default().to_string());
                fs::write(
                    output.join("webgl2-bindings.json"),
                    serde_json::to_vec_pretty(
                        &serde_json::json!({"samplers": mapping, "uniforms": uniforms}),
                    )?,
                )?;
                ("fragment.glsl", text.into_bytes())
            }
            "metal-fragment" => {
                let mut bindings = naga::back::msl::EntryPointResources::default();
                for (_, global) in module.global_variables.iter() {
                    if let Some(binding) = &global.binding {
                        let mut target = naga::back::msl::BindTarget::default();
                        match (binding.group, binding.binding) {
                            (0, 0) => target.texture = Some(0),
                            (0, 1) => {
                                target.sampler =
                                    Some(naga::back::msl::BindSamplerTarget::Resource(0))
                            }
                            (0, 2) => target.buffer = Some(0),
                            (1, 0) => target.buffer = Some(1),
                            _ => return Err("Unexpected Metal binding".into()),
                        }
                        bindings.resources.insert(binding.clone(), target);
                    }
                }
                let mut options = naga::back::msl::Options {
                    fake_missing_bindings: false,
                    lang_version: (2, 0),
                    ..Default::default()
                };
                options
                    .per_entry_point_map
                    .insert(ep.name.clone(), bindings);
                let (text, translation) =
                    naga::back::msl::write_string(&module, &info, &options, &Default::default())?;
                let index = module
                    .entry_points
                    .iter()
                    .position(|entry| entry.name == ep.name)
                    .ok_or("Missing Metal entry")?;
                backend_entry = translation
                    .entry_point_names
                    .into_iter()
                    .nth(index)
                    .ok_or("Missing translated Metal entry")?
                    .map_err(|e| format!("Metal entry point: {e:?}"))?;
                ("fragment.metal", text.into_bytes())
            }
            _ => return Err(format!("Unsupported required backend: {profile}").into()),
        };
        fs::write(output.join(filename), &bytes)?;
        variants.push(
            serde_json::json!({"profile": profile, "file": filename, "sha256": hash(&bytes), "entryPoint": backend_entry}),
        );
    }
    let manifest = serde_json::json!({
        "schema": 1, "abi": 1, "compiler": COMPILER, "assetId": definition.asset_id,
        "entryPoint": ep.name, "stage": "fragment", "sourceHash": hash(source.as_bytes()),
        "definitionHash": hash(&definition_bytes), "bindings": resources,
        "uniformSize": uniform_size, "fields": uniform_fields, "variants": variants,
    });
    fs::write(
        output.join("manifest.json"),
        serde_json::to_vec_pretty(&manifest)?,
    )?;
    if args[0] == "generate" || args[0] == "compile" {
        let mut generated = format!(
            "// <auto-generated/>\nnamespace Doroti.Generated;\npublic sealed class {}Parameters {{\n",
            definition.asset_id
        );
        for field in &uniform_fields {
            let ty = match field.scalar.as_str() {
                "f32" => "float",
                "i32" => "int",
                _ => "uint",
            };
            generated += &format!(" public {ty} {} {{ get; init; }}\n", field.name);
        }
        generated += &format!(
            " public global::Doroti.Ui.GpuEffectParameters Snapshot() {{ var data = new byte[{uniform_size}];\n"
        );
        for field in &uniform_fields {
            let method = match field.scalar.as_str() {
                "f32" => "WriteSingleLittleEndian",
                "i32" => "WriteInt32LittleEndian",
                _ => "WriteUInt32LittleEndian",
            };
            generated += &format!(
                " global::System.Buffers.Binary.BinaryPrimitives.{method}(global::System.MemoryExtensions.AsSpan(data, {}), {});\n",
                field.offset, field.name
            );
        }
        generated += " return new global::Doroti.Ui.GpuEffectParameters(data); }\n}\n";
        let literal = |bytes: &[u8]| {
            bytes
                .iter()
                .map(|b| b.to_string())
                .collect::<Vec<_>>()
                .join(",")
        };
        generated += &format!(
            "public static partial class Effects {{ public static global::Doroti.Ui.GpuEffectProgram @{} {{ get; }} = new(\"{}\", {uniform_size}",
            definition.asset_id, definition.asset_id
        );
        for variant in &variants {
            let profile = variant["profile"]
                .as_str()
                .ok_or("Missing variant profile")?;
            let ext = match profile {
                "vulkan-fragment" => "spv",
                "webgpu-fragment" => "wgsl",
                "webgl2-fragment" => "glsl",
                _ => "metal",
            };
            let fragment = fs::read(output.join(format!("fragment.{ext}")))?;
            let vertex = fs::read(output.join(format!("vertex.{ext}")))?;
            let binding_metadata = if ext == "glsl" {
                fs::read_to_string(output.join("webgl2-bindings.json"))?
            } else {
                "{}".into()
            };
            let metadata_literal = serde_json::to_string(&binding_metadata)?;
            generated += &format!(
                ", new global::Doroti.Ui.GpuEffectVariant(\"{profile}\", \"{}\", new byte[]{{{}}}, new byte[]{{{}}}, {metadata_literal})",
                variant["entryPoint"]
                    .as_str()
                    .ok_or("Missing variant entry")?,
                literal(&vertex),
                literal(&fragment)
            );
        }
        generated += "); }\n";
        fs::write(output.join("Parameters.g.cs"), generated)?;
    }
    Ok(())
}
