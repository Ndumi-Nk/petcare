// Hubs/VideoHub.cs
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class VideoHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
    private static readonly ConcurrentDictionary<string, string> UserGroups = new ConcurrentDictionary<string, string>();

    public override Task OnConnected()
    {
        var httpContext = Context.Request.GetHttpContext();
        var sessionId = httpContext.Request.QueryString["sessionId"];
        var userId = httpContext.Request.QueryString["userId"];
        var userType = httpContext.Request.QueryString["userType"]; // "trainer" or "user"

        if (!string.IsNullOrEmpty(sessionId))
        {
            Groups.Add(Context.ConnectionId, sessionId);
            Connections[Context.ConnectionId] = userId;
            UserGroups[userId] = sessionId;
        }

        return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        string userId;
        Connections.TryRemove(Context.ConnectionId, out userId);

        if (userId != null)
        {
            string groupId;
            UserGroups.TryRemove(userId, out groupId);
        }

        return base.OnDisconnected(stopCalled);
    }

    public void SendSignal(string signal, string targetUserId)
    {
        string targetConnectionId = null;
        foreach (var connection in Connections)
        {
            if (connection.Value == targetUserId)
            {
                targetConnectionId = connection.Key;
                break;
            }
        }

        if (targetConnectionId != null)
        {
            Clients.Client(targetConnectionId).receiveSignal(Context.ConnectionId, signal);
        }
    }

    public void JoinRoom(string sessionId, string userId)
    {
        Groups.Add(Context.ConnectionId, sessionId);
        Connections[Context.ConnectionId] = userId;
        UserGroups[userId] = sessionId;

        Clients.Group(sessionId).userConnected(userId);
    }

    public void LeaveRoom(string sessionId, string userId)
    {
        Groups.Remove(Context.ConnectionId, sessionId);
        Clients.Group(sessionId).userDisconnected(userId);
    }

}