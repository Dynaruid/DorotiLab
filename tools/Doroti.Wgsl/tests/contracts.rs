use std::{
    fs,
    path::PathBuf,
    sync::atomic::{AtomicU32, Ordering},
};
static NEXT: AtomicU32 = AtomicU32::new(0);
struct Fixture(PathBuf);
impl Fixture {
    fn new(source: &str, entry: &str, profiles: &[&str]) -> Self {
        let path = std::env::temp_dir().join(format!(
            "doroti-wgsl-{}-{}",
            std::process::id(),
            NEXT.fetch_add(1, Ordering::Relaxed)
        ));
        fs::create_dir(&path).unwrap();
        fs::write(path.join("test.wgsl"), source).unwrap();
        fs::write(path.join("test.effect.json"), serde_json::to_vec(&serde_json::json!({
            "schema": 1, "assetId": "Contract", "entryPoint": entry, "requiredBackends": profiles
        })).unwrap()).unwrap();
        Self(path)
    }
    fn run(&self) -> Result<(), Box<dyn std::error::Error>> {
        doroti_wgsl::run(vec![
            "compile".into(),
            self.0.join("test.wgsl").to_str().unwrap().into(),
            self.0.join("test.effect.json").to_str().unwrap().into(),
            self.0.join("out").to_str().unwrap().into(),
        ])
    }
}
impl Drop for Fixture {
    fn drop(&mut self) {
        fs::remove_dir_all(&self.0).unwrap();
    }
}
const FS: &str = "@fragment fn fs_main(@location(0) uv: vec2<f32>) -> @location(0) vec4<f32> { return vec4<f32>(uv, 0., 1.); }";

#[test]
fn translated_profiles_are_real_and_deterministic() {
    let fixture = Fixture::new(
        FS,
        "fs_main",
        &[
            "vulkan-fragment",
            "webgl2-fragment",
            "metal-fragment",
            "webgpu-fragment",
        ],
    );
    fixture.run().unwrap();
    let manifest = fs::read(fixture.0.join("out/manifest.json")).unwrap();
    fixture.run().unwrap();
    assert_eq!(
        manifest,
        fs::read(fixture.0.join("out/manifest.json")).unwrap()
    );
    assert!(
        fs::read_to_string(fixture.0.join("out/fragment.glsl"))
            .unwrap()
            .starts_with("#version 300 es")
    );
    assert!(
        !fs::read_to_string(fixture.0.join("out/fragment.metal"))
            .unwrap()
            .contains("fake0")
    );
}

#[test]
fn uniform_padding_golden() {
    let source = format!(
        "struct P {{ a: f32, v: vec3<f32>, m: mat3x3<f32>, values: array<vec4<u32>, 2>, signed: i32 }}\n@group(1) @binding(0) var<uniform> p: P;\n{FS}"
    );
    let fixture = Fixture::new(&source, "fs_main", &["vulkan-fragment"]);
    fixture.run().unwrap();
    let manifest: serde_json::Value =
        serde_json::from_slice(&fs::read(fixture.0.join("out/manifest.json")).unwrap()).unwrap();
    assert_eq!(manifest["uniformSize"], 128);
    for (name, offset) in [
        ("p_a", 0),
        ("p_v_0", 16),
        ("p_v_2", 24),
        ("p_m_0_0", 32),
        ("p_m_1_0", 48),
        ("p_m_2_2", 72),
        ("p_values_0_0", 80),
        ("p_values_1_3", 108),
        ("p_signed", 112),
    ] {
        let field = manifest["fields"]
            .as_array()
            .unwrap()
            .iter()
            .find(|f| f["name"] == name)
            .unwrap();
        assert_eq!(field["offset"], offset, "{name}");
    }
}

#[test]
fn missing_entry_and_reserved_binding_are_rejected() {
    assert!(
        Fixture::new(FS, "absent", &["vulkan-fragment"])
            .run()
            .is_err()
    );
    let source = format!("@group(0) @binding(0) var input: sampler;\n{FS}");
    assert!(
        Fixture::new(&source, "fs_main", &["vulkan-fragment"])
            .run()
            .is_err()
    );
}

#[test]
fn compute_and_unknown_profiles_are_rejected() {
    assert!(
        Fixture::new(
            "@compute @workgroup_size(1) fn cs_main() {}",
            "cs_main",
            &["webgl2-fragment"]
        )
        .run()
        .is_err()
    );
    assert!(Fixture::new(FS, "fs_main", &["unknown"]).run().is_err());
}

#[test]
fn frame_uniform_checks_members_not_only_total_size() {
    let source = format!(
        "struct Wrong {{ data: array<vec4<f32>, 2> }}\n@group(0) @binding(2) var<uniform> frame: Wrong;\n{FS}"
    );
    assert!(
        Fixture::new(&source, "fs_main", &["vulkan-fragment"])
            .run()
            .is_err()
    );
}
