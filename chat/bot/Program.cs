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

    public static async Task Main(string[] args)
    {
        var client = new StreamClient<Message>();

        await client.Connect(_server.Replace("http", "ws") + "/chat.Chat/StreamMessages", new Chat.Empty());

        while (client.State == WebSocketState.Open)
        {
            var message = await client.Read();
            _ = HandleMessageReceived(message);
        }

        Console.WriteLine($"Stream client closed with status {client.CloseStatus}, exiting...");
    }

    public static async Task HandleMessageReceived(Message message)
    {
        try
        {
            Console.WriteLine($"Message received: {message.Content}");
            if (message.Content == "!ping")
                await SendMessage("Pong!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in handle message received: {e}");
        }
    }

    public static async Task SendMessage(string message)
    {
        var messageReq = new Message() { Content = message };

        var res = await _client.HrpcUnaryAsync<Message, Empty>(_server + "/chat.Chat/SendMessage", messageReq);
    }
}