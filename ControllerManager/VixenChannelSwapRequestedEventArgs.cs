using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public class VixenChannelSwapRequestedEventArgs:EventArgs
    {
        public VixenChannelSwapRequestedEventArgs(IControllerOutput source, IControllerOutput destination)
        {
            Source = source;
            Destination = destination;
        }
        public IControllerOutput Source { get; set; }

        public IControllerOutput Destination { get; set; }
    }
}
