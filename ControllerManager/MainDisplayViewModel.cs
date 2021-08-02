
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using WPFCommon;
using ControllerManager.Interfaces;
using Vixen;

namespace ControllerManager
{
    class MainDisplayViewModel:ViewModelBase,IVixenSaveData,IIsDropable
    {
        private ControllerManager controllerManager = new ControllerManager();
        private String nameBase = "Controller";
        private XmlNode _dataNode;
        private ObservableCollection<IDisplayAbleObject> _deviceObjectList = new ObservableCollection<IDisplayAbleObject>();
        private ObservableCollection<IDisplayAbleObject> _channelList = new ObservableCollection<IDisplayAbleObject>();
        private EventSequence _sequence;

        public MainDisplayViewModel(EventSequence sequence,XmlNode dataNode)
        {
            //controllerManager.DisplayItems = displayObjects;
            //controllerManager.Channels = channels;
            _sequence = sequence;
            _dataNode = dataNode;
            Init();
        }

        private ObservableCollection<IDisplayAbleObject> _outputObjects;

        public ObservableCollection<IDisplayAbleObject> DisplayItems
        {
            get { return controllerManager.DisplayItems; }
            set { controllerManager.DisplayItems = value;}
        }

        private IEnumerable<IController> Controllers
        {
            get { return DisplayItems.OfType<IController>(); }
        }

        private ObservableCollection<IDisplayAbleObject> _channels;

        public ObservableCollection<IDisplayAbleObject> Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        private void Save()
        {
            //currently test data
            Controller controller = new Controller();
            controller.ChannelCount = 6;
            controller.Name = "TestController";

            VixenSerializer serializer = new VixenSerializer();
            serializer.Serialize(controller.GetType(),controller,_dataNode);
        }

        public void SaveToXML(XmlNode dataNode)
        {
            XmlNode controllerNode = Xml.GetEmptyNodeAlways(dataNode, "Controllers");
            foreach (IController controller in Controllers)
            {
                controller.SaveToXML(controllerNode);
            }
            dataNode.AppendChild(controllerNode);
        }

        private IDisplayAbleObject _selectedItem;

        public IDisplayAbleObject SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        #region init

        private void LoadDeviceObjects()
        {
            //for now, this is just the list of channels
            foreach (IDisplayAbleObject item in _channelList)
            {
                _deviceObjectList.Add(item);
            }
        }

        private void LoadVixenOutputs(List<Channel> vixenOutputs)
        {
            foreach (Channel channel in vixenOutputs)
            {
                _channelList.Add(new VixenNativeChannel(channel));
            }
        }

        private void Init()
        {
            if (_sequence.Profile != null)
            {
                LoadVixenOutputs(_sequence.Profile.OutputChannels);
            }
            else
            {
                LoadVixenOutputs(_sequence.Channels);
            }

            LoadDeviceObjects();

            controllerManager.DisplayItems = _deviceObjectList ;
            controllerManager.Channels = _channelList;

        }

        #endregion init

        #region commands

        RelayCommand<object> _addController;
        RelayCommand<Object> _testButton;
        RelayCommand<object> _saveCommand;

        public ICommand AddControllerCommand
        {
            get
            {
                if (_addController == null)
                {
                    _addController = new RelayCommand<object>(param => this.AddControllerExecuted(), param => this.CanExecuteIfCanAddControllers());
                }
                return _addController;
            }
        }

        public ICommand TestButtonCommand
        {
            get
            {
                if (_testButton == null)
                {
                    _testButton = new RelayCommand<object>(param => this.TestButtonExecuted(), param => this.CanExecuteAlways());
                }
                return _testButton;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand<object>(param => this.SaveCommandExecuted(), param => this.CanExecuteAlways());
                }
                return _saveCommand;
            }
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

        public void SaveCommandExecuted()
        {
            SaveToXML(_dataNode);
        }

        public void TestButtonExecuted()
        {
            MessageBox.Show("Test button pressed.");

        }

        public void AddControllerExecuted()
        {
            controllerManager.AddController("Controller", 16);
        }

        public bool CanExecuteIfCanAddControllers()
        {
            return controllerManager.CanAddControllers;
        }

        public bool CanExecuteAlways()
        {
            return true;
        }

        public void DroppedItemExecuted(DragDropParameters parameters)
        {
            SwapChannels(parameters.SourceOutput, parameters.DestnationOutput);
        }

        public bool CanExecuteDroppedItem()
        {
            return true;
        }

        private void SwapChannels(IControllerOutput source, IControllerOutput destination)
        {
            Messenger.Instance.SendVixenChannelSwapRequest(source, destination);
        }





        #endregion commands


    }
}
