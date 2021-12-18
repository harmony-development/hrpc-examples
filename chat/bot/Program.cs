using System.Net.WebSockets;
using Chat;

using Generated;

namespace bot;

public static class Program
{
    public static ChatClient client = new("http://localhost:2289");

    public static async Task Main(string[] args)
    {
        var stream = await client.StreamMessages(new Empty());

        while (stream.State == WebSocketState.Open)
        {
            var message = await stream.Read();
            _ = HandleMessageReceived(message);
        }

        Console.WriteLine($"Stream client closed with status {stream.CloseStatus}, exiting...");
    }

    public static async Task HandleMessageReceived(Message message)
    {
        try
        {
            Console.WriteLine($"Message received: {message.Content}");
            if (message.Content == "!ping")
                await client.SendMessage(new Message() { Content = "Pong!" });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in handle message received: {e}");
        }
    }
}