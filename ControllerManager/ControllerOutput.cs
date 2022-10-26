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
        private Brush _blueBrush = new SolidColorBrush(Colors.Blue);
        private Brush _yellowBrush = new SolidColorBrush(Colors.Yellow);
        private Brush _whiteBrush = new SolidColorBrush(Colors.White);
        
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

        private IDisplayAbleObject _selectedItem;
        public IDisplayAbleObject SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                System.Windows.MessageBox.Show("In ControllerOutput");
                OnPropertyChanged("SelectedItem");
            }
        }

        public ControllerOutput(IVixenChannel vixenOutput, int output, bool isEnabled, bool isBroken)
        {
            IsEnabled = isEnabled;
            IsBroken = isBroken;
            _vixenOutput = vixenOutput;
            Output = output;
        }

        public bool IsEnabled
        {
            get { return VixenChannel.VixenChannel.Enabled; }
            set { 
                VixenChannel.VixenChannel.Enabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool _isBroken;

        public bool IsBroken
        {
            get { return _isBroken; }
            set
            {
                _isBroken = value;
                OnPropertyChanged("IsBroken");
            }
        }

        public string Name
        {
            get { return _vixenOutput.Name; }
            set
            {
                _vixenOutput.Name = value;
            }
        }

        public int VixenChannelOutputNumber
        {
            get { return _vixenOutput.VixenChannel.OutputChannel + 1; }
        }

        public IVixenChannel VixenChannel
        {
            get { return _vixenOutput; }
            set
            {
                _vixenOutput = value;
                RaiseOnPropertyChanged(new string[]{"Name","IsEnabled","ChannelColor"});
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
            set 
            { 
                dragDropState = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public Brush BackgroundColor
        {
            get
            {
                
                switch (DragDropState)
                {
                    case DragDropStates.DragItem:
                        return _blueBrush;
                    case DragDropStates.DragTarget:
                        return _yellowBrush;
                    default:
                        return _whiteBrush;
                }
            }
        }

        public Brush ChannelColor
        {
            get { return new SolidColorBrush(ColorUtil.WpfColorFromDrawingColor(VixenChannel.VixenChannel.Color)); }
        }

        public void RefreshChannelColor()
        {
            OnPropertyChanged("ChannelColor");
        }

        public int VixenChannelIndex
        {
            get { return this.VixenChannel.VixenChannel.OutputChannel; }
            set { OnPropertyChanged("VixenChannelIndex"); }
        }

    }
}
