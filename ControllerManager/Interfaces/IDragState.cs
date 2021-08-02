using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControllerManager.Interfaces
{
    public interface IDragState
    {
        DragDropStates DragDropState { get; set; }
    }
}
