using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public class DragDropParameters
    {
        public IControllerOutput SourceOutput { get; set; }
        public IControllerOutput DestnationOutput { get; set; }
    }
}
