using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen;

namespace ControllerManager
{
    public interface IVixenChannel:IDisplayAbleObject
    {
        Channel VixenChannel { get; set; }
    }
}
