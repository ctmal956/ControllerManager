using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public static class Utility
    {
        public static void SwapVixenChannels(ObservableCollection<IControllerOutput> collection, int sourceIndex, int destinationIndex)
        {
            IVixenChannel tmp = collection[sourceIndex].VixenChannel;
            collection[sourceIndex].VixenChannel = collection[destinationIndex].VixenChannel;
            collection[destinationIndex].VixenChannel = tmp;
        }
    }
}
