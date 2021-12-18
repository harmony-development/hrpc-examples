# `chat`

A chat application example. 

You will need the [Rust toolchain installed](https://rustup.rs) to run the server.
If you want to run the web client, you will need [`pnpm`](https://pnpm.io).
If you want to run the bot, you will need the [dotnet sdk](https://dot.net) and hrpc codegen tools.

To run the server, navigate to `server` and run:
```console
$ cargo run
```

## Web client

To run the web client, navigate to `client` and run:
```console
$ pnpm i && pnpm dev
```

## CLI client

To run the CLI client, navigate to `tui-client` and run:
```console
$ cargo run
```

## Bot

To run the chat bot, navigate to `bot` and run:
```console
$ buf generate
$ dotnet run
```