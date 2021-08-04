﻿using System;
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

    }
}
