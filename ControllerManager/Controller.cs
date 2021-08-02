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
using WPFCommon;
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

        

        public Controller()
        {
            Outputs = new ObservableCollection<IControllerOutput>();
            IsExpanded = false;
        }


        #endregion constructors




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


        private void SwapChannels(IControllerOutput source, IControllerOutput destination)
        {
            Messenger.Instance.SendVixenChannelSwapRequest(source, destination);
        }

        public ObservableCollection<IControllerOutput> Outputs { get; set; }

        public int ChannelStartIndex { get; set; }

        //Point _dragStart = new Point();
        //public void ListBox_PreviewMouseLeftButtonDown(Point mousePosition)
        //{
        //    _dragStart = mousePosition;
        //}

        public void DroppedItemExecuted(DragDropParameters parameters)
        {
            SwapChannels(parameters.SourceOutput, parameters.DestnationOutput);
        }

        public bool CanExecuteDroppedItem()
        {
            return true;
        }

        public void SaveToXML(XmlNode dataNode)
        {
            XmlNode node = Xml.GetEmptyNodeAlways(dataNode, "Controller");
            Xml.SetAttribute(node, "Name", Name);
            Xml.SetAttribute(node, "ChannelCount", ChannelCount.ToString());
            Xml.SetAttribute(node, "Location", Location);
            XmlNode outputNode =  Xml.GetEmptyNodeAlways(node, "Outputs");
            foreach (IControllerOutput output in Outputs)
            {
                output.SaveToXML(outputNode);
            }
        }

        
        public IControllerOutput SelectedOutput
        {
            get { return _selectedOutput; }
            set { _selectedOutput = value; }
        }

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

        public DragDropStates DragDropState { get; set; } = DragDropStates.None;

        private Color _backgroundColor;

        

    }
}
