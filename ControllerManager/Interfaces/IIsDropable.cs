using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControllerManager.Interfaces
{
    public interface IIsDropable:IDragState
    {
        ICommand DroppedItem { get; }

        void DroppedItemExecuted(DragDropParameters parameter);

        bool CanExecuteDroppedItem();

    }
}
