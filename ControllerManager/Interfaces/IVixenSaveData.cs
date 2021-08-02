using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ControllerManager.Interfaces
{
    public interface IVixenSaveData
    {
        void SaveToXML(XmlNode dataNode);
    }
}
