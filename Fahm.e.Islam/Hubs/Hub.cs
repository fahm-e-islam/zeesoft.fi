using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using ZeeSoft.ClassRoomJson;

namespace WebApplication2.SignalR
{
    public enum SignalTypes
    {
        NewMessageCreated,
        NewUserLoggedIn
    }
    public class FIHub : Hub
    {

        public void Send(string name, string messageJson)
        {
            var type=(SignalTypes)Enum.Parse(typeof(SignalTypes), name);

            switch (type)
            {
                case SignalTypes.NewMessageCreated:
                    var msg = JsonConvert.DeserializeObject<Message>(messageJson);
                    var receivers=String.Join(",", msg.RecipientIds);
                    Clients.All.NewMessageCreated(receivers, msg);

                    break;
                default:
                    break;
            }
            
        }
    }
}