using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public interface IControllerOutput:IDisplayAbleObject,IVixenSaveData
    {
        bool IsEnabled { get; set; }
        bool IsBroken { get; set; }
        IVixenChannel VixenChannel { get; set; }
        int Output { get; set; }

    }
}
