// src/build.rs
fn main() {
    // The path here is the path to our protocol file
    // which we copied in the previous step
    hrpc_build::compile_protos("protocol/chat.proto").expect("could not compile the proto");
}
