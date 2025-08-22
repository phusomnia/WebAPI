using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;


namespace WebAPI.Features.RealTime;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _userGroup = new(); 

    public async Task JoinGroup(string user, string groupName)
    {
        // TODO: store id channel to db
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _userGroup[Context.ConnectionId] = groupName;
        await Clients.Group(groupName).SendAsync("ReceiveMsg", "System", $"{user} join {groupName}");
    }

    public async Task SendMessageToGroup(String user, String message)
    {
        if (_userGroup.TryGetValue(Context.ConnectionId, out var groupName))
        {
            Console.WriteLine(groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMsg", user, message);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveMsg", "System", "You are not in any group");
        }
    }

    public async Task SendMessage(String user, String message)
    {
        await Clients.All.SendAsync("ReceiveMsg", user, message);
        Console.WriteLine("{0}: {1}", user, message);
    }

    public async Task SendNotification(String message)
    {
        await Clients.All.SendAsync("Recieve noti: ", message);
    }
}