using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsciiRPG.Network;

public class LanSession
{
    public async Task StartHostAsync(int port, CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"LAN host started on port {port}");

        while (!token.IsCancellationRequested)
        {
            if (!listener.Pending())
            {
                await Task.Delay(100, token);
                continue;
            }

            var client = await listener.AcceptTcpClientAsync(token);
            _ = HandleClientAsync(client, token);
        }
    }

    public async Task JoinAsync(string host, int port, string playerName)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        var stream = client.GetStream();
        var msg = Encoding.UTF8.GetBytes($"READY:{playerName}");
        await stream.WriteAsync(msg);
        Console.WriteLine("Connected to host. Ready flag sent.");
    }

    private static async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        using var c = client;
        var buffer = new byte[1024];
        var stream = c.GetStream();
        var read = await stream.ReadAsync(buffer, token);
        var text = Encoding.UTF8.GetString(buffer, 0, read);
        Console.WriteLine($"LAN event: {text}");
    }
}
