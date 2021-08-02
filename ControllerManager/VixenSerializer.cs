using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using ControllerManager.CustomAttributes;

namespace ControllerManager
{
    public class VixenSerializer
    {
        public void Serialize(Type t,object dataObject,XmlNode dataNode)
        {
            var type = dataObject.GetType();
            var props = type.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(SaveDataAttribute)));

            foreach (var prop in props)
            {
                string name = prop.Name;
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    WriteNode(dataObject, prop, Vixen.Xml.GetNodeAlways(dataNode,type.Name));
                }
            }
        }

        private void WriteNode(object dataObject, PropertyInfo pinfo,XmlNode node)
        {
            MessageBox.Show(String.Format("{0}:{1}", pinfo.Name, pinfo.GetValue(dataObject, null)));
            string value = pinfo.GetValue(dataObject, null).ToString();
            if (value == null)
                value = "";
            Vixen.Xml.SetNewValue(node, pinfo.Name, pinfo.GetValue(dataObject,null).ToString());
        }

        
    }
}
