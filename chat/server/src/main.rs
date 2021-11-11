// src/main.rs

// Our chat package generated code
pub mod chat {
    // This imports all the generated code for you
    hrpc::include_proto!("chat");
}

use chat::{chat_server::*, *};
use hrpc::server::prelude::*;
use hrpc::server::transport::http::Hyper;
use tokio::sync::broadcast;
use tower_http::cors::CorsLayer;

pub struct ChatService {
    // The sender half of our broadcast channel.
    //
    // We will use it's `.subscribe()` method to get a
    // receiver when a client connects.
    message_broadcast: broadcast::Sender<Message>,
}

impl ChatService {
    // Creates a new `ChatService`
    fn new() -> Self {
        // Create a broadcast channel with a maximum 100
        // amount of items that can be pending. This
        // doesn't matter in our case, so the number is
        // arbitrary.
        let (tx, _) = broadcast::channel(100);
        Self {
            message_broadcast: tx,
        }
    }
}

impl Chat for ChatService {
    // This corresponds to the SendMessage endpoint
    //
    // `handler` is a Rust macro that is used to transform
    // an `async fn` into a properly typed hRPC trait method.
    #[handler]
    async fn send_message(&self, request: Request<Message>) -> ServerResult<Response<Empty>> {
        // Extract the chat message from the request
        let message = request.into_message().await?;

        // Try to broadcast the chat message across the channel
        // if it fails return an error
        if self.message_broadcast.send(message).is_err() {
            return Err(HrpcError::new_internal_server_error(
                "couldn't broadcast message",
            ));
        }

        Ok((Empty {}).into_response())
    }

    // This corresponds to the StreamMessages endpoint
    #[handler]
    async fn stream_messages(
        &self,
        _request: Request<()>,
        socket: Socket<Message, Empty>,
    ) -> ServerResult<()> {
        // Subscribe to the message broadcaster
        let mut message_receiver = self.message_broadcast.subscribe();

        // Poll for received messages...
        while let Ok(message) = message_receiver.recv().await {
            // ...and send them to client.
            socket.send_message(message).await?;
        }

        Ok(())
    }
}

// `tokio::main` is a Rust macro that converts an `async fn`
// `main` function into a synchronous `main` function, and enables
// you to use the `tokio` runtime. The runtime we use is the
// multithreaded runtime, which is what we want (the `rt-threaded`
// feature on `tokio` was needed for this).
#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Create our chat service
    let service = ChatServer::new(ChatService::new());

    // Create our transport that we will use to serve our service
    let transport = Hyper::new("127.0.0.1:2289")?;

    // Layer our transport for use with CORS.
    // Since this is specific to HTTP, we use the transport's layer method.
    //
    // Note: A "layer" can simply be thought of as a middleware!
    let transport = transport.layer(CorsLayer::permissive());

    // Serve our service with our transport
    transport.serve(service).await?;

    Ok(())
}
