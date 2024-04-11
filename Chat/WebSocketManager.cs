using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Program.cs remains largely the same as your provided snippet.

// WebSocketManager.cs
public class WebSocketManager
{
    private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public WebSocket GetSocketById(string id) => _sockets[id];

    public ConcurrentDictionary<string, WebSocket> GetAllSockets() => _sockets;

    public string AddSocket(WebSocket socket)
    {
        string id = Guid.NewGuid().ToString();
        _sockets.TryAdd(id, socket);
        return id;
    }

    public async Task RemoveSocketAsync(string id)
    {
        _sockets.TryRemove(id, out var socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketManager", CancellationToken.None);
    }
}