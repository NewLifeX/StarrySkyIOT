using InterconnectionManagementAPP.Enums;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterconnectionManagementAPP.Models
{
    public class MessageEvent : PubSubEvent<MessageModel>
    {
    }
    public class MessageModel
    {
        public EventTypes Command { get; set; }
        public object Data { get; set; }
    }
}
