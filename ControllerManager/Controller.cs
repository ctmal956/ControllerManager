using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPFCommon;
using ControllerManager.Interfaces;
using System.Xml;
using System.Xml.Serialization;
using ControllerManager.ExtensionMethods;
using ControllerManager.CustomAttributes;
using Vixen;
using System.Windows.Media;

namespace ControllerManager
{
    public class Controller : ViewModelBase,IController,IIsDropable
    {
        #region private members

        private string _name;
        private int _channelCount;
        private string _location;
        private bool _isExpanded;
        private IControllerOutput _selectedOutput;


        #endregion private members

        #region constructors

        public Controller(string name, int outputCount)
        {
            Init();

            Name = name;
            ChannelCount = outputCount;
            for (int i = 0; i < ChannelCount; i++)
            {
                Outputs.Add(new ControllerOutput(i + 1));
            }

        }

        public Controller(XmlNode dataNode)
        {
            Init();
            Name = dataNode.Attributes["Name"].Value;
            ChannelCount = Convert.ToInt32(dataNode.Attributes["ChannelCount"].Value);
          
            Location = dataNode.Attributes["Location"].Value;
            XmlNode outputs = Xml.GetNodeAlways(dataNode, "Outputs");
            if (outputs.HasChildNodes)
            {
                foreach (XmlNode output in outputs.ChildNodes)
                {
                    Outputs.Add(new ControllerOutput(output));
                }

            }
        }

        private void Init()
        {
            Outputs = new ObservableCollection<IControllerOutput>();
            IsExpanded = false;
        }

        private void InitOutputs(int outputCount)
        {

        }


        #endregion constructors

        #region Public Properties

        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int ChannelCount
        {
            get { return _channelCount; }
            set
            {
                _channelCount = value;
                OnPropertyChanged("ChannelCount"); 
            }
        }
      
        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged("Location"); 
            }
        }
        
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        public IControllerOutput SelectedOutput
        {
            get { return _selectedOutput; }
            set { _selectedOutput = value; }
        }

        public ObservableCollection<IControllerOutput> Outputs { get; set; }

        public int ChannelStartIndex { get; set; }

        #endregion


        private void SwapChannels(IControllerOutput source, IControllerOutput destination)
        {
            Messenger.Instance.SendVixenChannelSwapRequest(source, destination);
        }


        #region Commands

        RelayCommand<object> _droppedItem;
        public ICommand DroppedItem
        {
            get
            {
                if (_droppedItem == null)
                {
                    _droppedItem = new RelayCommand<object>(param => DroppedItemExecuted(param as DragDropParameters), param => CanExecuteDroppedItem());
                }
                return _droppedItem;
            }
        }

        #endregion

        public void DroppedItemExecuted(DragDropParameters parameters)
        {
            SwapChannels(parameters.SourceOutput, parameters.DestnationOutput);
            parameters.DestnationOutput.RefreshChannelColor();
            parameters.SourceOutput.RefreshChannelColor();
        }

        public bool CanExecuteDroppedItem()
        {
            return true;
        }

        public void SaveToXML(XmlNode dataNode)
        {
            XmlNode newNode = dataNode.CreateNewNode("Controller");
            Xml.SetAttribute(newNode, "Name", Name);
            Xml.SetAttribute(newNode, "ChannelCount", ChannelCount.ToString());
            Xml.SetAttribute(newNode, "Location", Location);
            XmlNode outputNode =  Xml.GetEmptyNodeAlways(newNode, "Outputs");
            foreach (IControllerOutput output in Outputs)
            {
                output.SaveToXML(outputNode);
            }
            dataNode.AppendChild(newNode);
        }

        public DragDropStates DragDropState { get; set; } = DragDropStates.None;

    }
}
