using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControllerManager;
using ControllerManager.Interfaces;

namespace WPFCommon
{
    public sealed class Messenger
    {
        private static readonly Messenger instance = new Messenger();

        private Messenger() { }

        public static Messenger Instance
        {
            get
            {
                return instance;
            }
        }

        //declaring EventHandler
        public event EventHandler<VixenChannelSwapRequestedEventArgs> VixenChannelSwapRequested;

        //raising the event for a property

        public void SendVixenChannelSwapRequest(IControllerOutput source, IControllerOutput destination)
        {
            VixenChannelSwapRequested?.Invoke(this, new VixenChannelSwapRequestedEventArgs(source, destination));
        }
    }
}
