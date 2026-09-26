fn main() {
    if let Err(error) = doroti_wgsl::run(std::env::args().skip(1).collect()) {
        eprintln!("error DOROTIWGSL001: {error}");
        std::process::exit(1);
    }
}
