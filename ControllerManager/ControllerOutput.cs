using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml;
using Vixen;
using WPFCommon;

namespace ControllerManager
{
    public class ControllerOutput : ViewModelBase, IControllerOutput
    {
        private IVixenChannel _vixenOutput;
        public ControllerOutput(IVixenChannel vixenOutput,int output) : this(vixenOutput, output, true, false) { }
        private int _output;
        
        public int Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged("Output");
            }
        }

        public ControllerOutput(XmlNode dataNode)
        {
            Output = Convert.ToInt32(dataNode.InnerText);
            IsBroken = Convert.ToBoolean(dataNode.Attributes["IsBroken"].Value);
        }

        public ControllerOutput(int channelNumber)
        {
            Output = channelNumber;
        }

        public ControllerOutput(IVixenChannel vixenOutput, int output, bool isEnabled, bool isBroken)
        {
            IsEnabled = isEnabled;
            IsBroken = isBroken;
            _vixenOutput = vixenOutput;
            Output = output;
        }


        public bool IsEnabled { get; set; }
        public bool IsBroken { get; set; }
        public string Name
        {
            get { return _vixenOutput.Name; }
            set
            {
                _vixenOutput.Name = value;
            }
        }
        public IVixenChannel VixenChannel
        {
            get { return _vixenOutput; }
            set
            {
                _vixenOutput = value;
                RaiseOnPropertyChanged(new string[]{"Name","IsEnabled"});
            }
        }

        private void RaiseOnPropertyChanged(string [] properties)
        {
            foreach (var prop in properties)
            {
                OnPropertyChanged(prop);
            }
        }

        public void SaveToXML(XmlNode dataNode)
        {
            XmlNode newNode = Xml.SetNewValue(dataNode, "ControllerOutput", Output.ToString());
            Xml.SetAttribute(newNode, "IsBroken", IsBroken.ToString());
            
        }

        public override string ToString()
        {
            return Name;
        }

        private DragDropStates dragDropState;

        public DragDropStates DragDropState
        {
            get { return dragDropState; }
            set { dragDropState = value; }
        }

        public Color BackgroundColor
        {
            get
            {
                switch (DragDropState)
                {
                    case DragDropStates.DragItem:
                        return Colors.Orange;
                        break;
                    case DragDropStates.DragTarget:
                        return Colors.Yellow;
                        break;
                    default:
                        return Colors.Purple;
                        break;
                }
            }
        }

    }
}
