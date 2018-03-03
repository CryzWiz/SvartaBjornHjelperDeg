﻿using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Hubs
{
    public class TestChat : Hub
    {
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public override async Task OnConnectedAsync()
        {
            // Map connections using in-memory ConnectionMapping
            // TODO: Name is set to be connectionId, to be changed later..
            string name = Context.ConnectionId;
            _connections.Add(name, Context.ConnectionId);

            await Clients.All.InvokeAsync("broadcastMessage", $"{Context.ConnectionId} joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // Map connections using in-memory ConnectionMapping
            // TODO: Name is set to be connectionId, to be changed later..
            string name = Context.ConnectionId;
            _connections.Remove(name, Context.ConnectionId);


            await Clients.All.InvokeAsync("broadcastMessage", $"{Context.ConnectionId} left");
        }

        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("broadcastMessage", $"{Context.ConnectionId}: {message}");
        }

        public Task SendToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).InvokeAsync("Send", $"{Context.ConnectionId}@{groupName}: {message}");
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).InvokeAsync("Send", $"{Context.ConnectionId} joined {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).InvokeAsync("broadcastMessage", $"{Context.ConnectionId} left {groupName}");
        }

        public Task Echo(string message)
        {
            return Clients.Client(Context.ConnectionId).InvokeAsync("broadcastMessage", $"{Context.ConnectionId}: {message}");
        }
    }
}
