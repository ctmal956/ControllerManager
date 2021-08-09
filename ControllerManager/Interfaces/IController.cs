using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public interface IController:IDisplayAbleObject,IVixenSaveData
    {
        int ChannelCount { get; set; }
        ObservableCollection<IControllerOutput> Outputs { get; set; }
        bool IsExpanded { get; set; }
        string Location { get; set; }
    }
}
