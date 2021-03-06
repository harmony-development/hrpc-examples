// @generated by protobuf-ts 2.0.7 with parameter generate_dependencies,long_type_string
// @generated from protobuf file "chat.proto" (package "chat", syntax proto3)
// tslint:disable
import type { RpcTransport } from "@protobuf-ts/runtime-rpc";
import type { ServiceInfo } from "@protobuf-ts/runtime-rpc";
import { Chat } from "./chat";
import type { ServerStreamingCall } from "@protobuf-ts/runtime-rpc";
import { stackIntercept } from "@protobuf-ts/runtime-rpc";
import type { Empty } from "./chat";
import type { Message } from "./chat";
import type { UnaryCall } from "@protobuf-ts/runtime-rpc";
import type { RpcOptions } from "@protobuf-ts/runtime-rpc";
/**
 * Chat service.
 *
 * @generated from protobuf service chat.Chat
 */
export interface IChatClient {
    /**
     * Endpoint to send a chat message.
     *
     * @generated from protobuf rpc: SendMessage(chat.Message) returns (chat.Empty);
     */
    sendMessage(input: Message, options?: RpcOptions): UnaryCall<Message, Empty>;
    /**
     * Endpoint to stream chat messages.
     *
     * @generated from protobuf rpc: StreamMessages(chat.Empty) returns (stream chat.Message);
     */
    streamMessages(input: Empty, options?: RpcOptions): ServerStreamingCall<Empty, Message>;
}
/**
 * Chat service.
 *
 * @generated from protobuf service chat.Chat
 */
export class ChatClient implements IChatClient, ServiceInfo {
    typeName = Chat.typeName;
    methods = Chat.methods;
    options = Chat.options;
    constructor(private readonly _transport: RpcTransport) {
    }
    /**
     * Endpoint to send a chat message.
     *
     * @generated from protobuf rpc: SendMessage(chat.Message) returns (chat.Empty);
     */
    sendMessage(input: Message, options?: RpcOptions): UnaryCall<Message, Empty> {
        const method = this.methods[0], opt = this._transport.mergeOptions(options);
        return stackIntercept<Message, Empty>("unary", this._transport, method, opt, input);
    }
    /**
     * Endpoint to stream chat messages.
     *
     * @generated from protobuf rpc: StreamMessages(chat.Empty) returns (stream chat.Message);
     */
    streamMessages(input: Empty, options?: RpcOptions): ServerStreamingCall<Empty, Message> {
        const method = this.methods[1], opt = this._transport.mergeOptions(options);
        return stackIntercept<Empty, Message>("serverStreaming", this._transport, method, opt, input);
    }
}
