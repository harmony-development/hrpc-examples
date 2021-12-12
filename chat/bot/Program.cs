using System.Buffers;
using System.Linq;
using System.Net.WebSockets;
using Chat;

using Hrpc;

namespace bot;

public static class Program
{
    private static HttpClient _client = new();
    private static string _server = "http://localhost:2289";
    private static ClientWebSocket _socket = new();

    public static async Task Main(string[] args)
    {
        await _socket.ConnectAsync(new Uri(_server.Replace("http", "ws") + "/chat.Chat/StreamMessages"), default);

        // send initial Empty message
        await _socket.SendAsync(Proto.Marshal(new Chat.Empty()).AsMemory(), WebSocketMessageType.Binary, true, default);

        while (_socket.State == WebSocketState.Open)
        {
            using var stream = new MemoryStream();
            using var buf = MemoryPool<byte>.Shared.Rent();
            var msg = await _socket.ReceiveAsync(buf.Memory, default);
            stream.Write(buf.Memory.Span.Slice(0, msg.Count));
            while (!msg.EndOfMessage)
            {
                msg = await _socket.ReceiveAsync(buf.Memory, default);
                stream.Write(buf.Memory.Span.Slice(0, msg.Count));
            }

            if (msg.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine($"Got close message with status {_socket.CloseStatus}, exiting...");
                return;
            }

            _ = HandleMessageReceived(stream.GetBuffer());
        }
    }

    public static async Task HandleMessageReceived(byte[] message)
    {
        try
        {
            // h
            int endIndex = message.Length - 1;
            while (endIndex >= 0 && message[endIndex] == 0)
                endIndex--;

            byte[] result = new byte[endIndex + 1];
            Array.Copy(message, 0, result, 0, endIndex + 1);

            var parsed = Proto.Unmarshal<Message>(result);
            Console.WriteLine($"Message received: {parsed.Content}");
            if (parsed.Content == "!ping")
                await SendMessage("Pong!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in handle message received: {e}");
        }
    }

    public static async Task SendMessage(string message)
    {
        var messageReq = new Message()
        {
            Content = message
        };

        var content = new ByteArrayContent(Proto.Marshal(messageReq));
        content.Headers.Add("Content-Type", "application/hrpc");

        var httpRes = await _client.PostAsync("http://localhost:2289/chat.Chat/SendMessage", content);

        var res = Proto.Unmarshal<Chat.Empty>(await httpRes.Content.ReadAsByteArrayAsync(), (int)httpRes.StatusCode);
    }
}