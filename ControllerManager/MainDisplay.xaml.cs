using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using ControllerManager.Interfaces;
using Vixen;
using System.IO;
using WPFCommon;
using System.ComponentModel;

namespace ControllerManager
{
    /// <summary>
    /// Interaction logic for MainDisplay.xaml
    /// </summary>
    public partial class MainDisplay : Window,INotifyPropertyChanged,IIsDropable
    {
        #region global members

        private ControllerManager _controllerManager = new ControllerManager();
        private XmlNode _dataNode;
        private ObservableCollection<IDisplayAbleObject> _deviceObjectList = new ObservableCollection<IDisplayAbleObject>();
        private ObservableCollection<IDisplayAbleObject> _channelList = new ObservableCollection<IDisplayAbleObject>();
        private EventSequence _sequence;
        private string _sequenceBackupPath = System.IO.Path.Combine(Paths.SequencePath, "Backup");
        private XmlDocument _profileDocument;
        private string _profileFilename;
        private string _profileExtension = ".cmp";
        private string _profileFullPath;
        private ObservableCollection<IDisplayAbleObject> _channels;
        private IDisplayAbleObject _selectedItem;
        private bool _dataNotSaved = false;
        private bool _pendingSave = false;

        #endregion region gloabl memebers


        #region Constructors

        public MainDisplay(EventSequence sequence,XmlNode dataNode)
        {
            InitializeComponent();
            this.DataContext = this;
            _sequence = sequence;
            _dataNode = dataNode;
            Init();
        }

        #endregion contructors

        #region init

        private void LoadDeviceObjects()
        {
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

        /// <summary>
        /// Loads saved data from the provided xmlnode
        /// </summary>
        /// <param name="savedDataNode"></param>
        private void LoadSavedDataFromNode(XmlNode savedDataNode)
        {
            //XmlNode controllers = Xml.GetNodeAlways(savedDataNode, "Controllers");
            XmlNode controllers = savedDataNode.SelectSingleNode("Controllers");
            if (controllers != null)
            {
                if (controllers.HasChildNodes)
                {
                    foreach (XmlNode controller in controllers.ChildNodes)
                    {
                        _controllerManager.AddController(new Controller(controller));
                    }
                }
            }
        }

        private bool PrepareProfileNode()
        {
            _profileFilename = _sequence.Profile + _profileExtension;
            _profileFullPath = System.IO.Path.Combine(Paths.ProfilePath, _profileFilename);

            if (File.Exists(_profileFullPath))
            {
                if (_profileDocument == null)
                {

                    _profileDocument = Xml.LoadDocument(_profileFullPath);

                }
                _dataNode = Xml.GetNodeAlways(_profileDocument, "Profile");
                return true;
            }
            return false;

        }

        private void LoadSavedData()
        {
            if (_sequence.Profile != null)
            {
                if (PrepareProfileNode())
                    LoadSavedDataFromNode(_dataNode);
            }
            else
            {
                LoadSavedDataFromNode(_dataNode);
            }
        }

        /// <summary>
        /// Determines if data is to be saved/restored in a profile or in the sequence.
        /// And sets up all variables.
        /// </summary>
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

            _controllerManager.DisplayItems = _deviceObjectList;
            _controllerManager.Channels = _channelList;

            LoadSavedData();

        }

        #endregion Init

        #region properties

        public bool IsDirty
        {
            get { return _dataNotSaved; }
            set
            {
                _dataNotSaved = value;
                OnPropertyChanged("IsDirty");
            }
        }


        public IDisplayAbleObject SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<IDisplayAbleObject> DisplayItems
        {
            get { return _controllerManager.DisplayItems; }
            set { _controllerManager.DisplayItems = value; }
        }

        private IEnumerable<IController> Controllers
        {
            get { return DisplayItems.OfType<IController>(); }
        }

        public ObservableCollection<IDisplayAbleObject> Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        #endregion properties

        #region save

        private void Save()
        {

            if (_sequence.Profile != null)
            {
                if (_profileDocument == null)
                {
                    _profileDocument = Xml.CreateXmlDocument("Profile");   
                }
                SaveToXML(_profileDocument.DocumentElement);
                _profileDocument.Save(System.IO.Path.Combine(Paths.ProfilePath, _profileFilename));
            }
            else
            {
                SaveToXML(_dataNode);
            }

            if (_controllerManager.IsChannelOrderDirty)
            {
                if (_sequence.Profile != null)
                {
                    //Save to profile
                    _sequence.Profile.OutputChannels = _controllerManager.GetNewOutputList();
                    AutoBackupProfile();
                    _sequence.Profile.SaveToFile();
                    _sequence.Profile.Reload();
                }
            }

            this.IsDirty = true;
        }

        private void AutoBackupProfile()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateBackupFolder());   //backup directory
            sb.Append(@"\");
            sb.Append(ExtractFilename(_sequence.Profile.FileName));  //full path at this point
            sb.Append("_");  //add extension to provide uniqueness
            sb.Append(Guid.NewGuid().ToString());
            BackupProfile(sb.ToString());
        }

        private String CreateBackupFolder()
        {
            String directory = Paths.ProfilePath + @"\Backup";
            if (!(System.IO.Directory.Exists(directory)))
                System.IO.Directory.CreateDirectory(directory);
            return directory;
        }

        private bool BackupProfile(String destPath)
        {
            System.IO.File.Copy(_sequence.Profile.FileName, destPath);
            return true;
        }

        private String ExtractFilename(String Path)
        {
            int index = Path.LastIndexOf(@"\");
            if (index != -1)
            {
                return Path.Remove(0, index + 1);
            }
            return Path;
        }

        public void SaveToXML(XmlNode dataNode)
        {
            XmlNode controllerNode = Xml.GetEmptyNodeAlways(dataNode, "Controllers");
            foreach (IController controller in Controllers)
            {
                controller.SaveToXML(controllerNode);
            }
            dataNode.AppendChild(controllerNode);
            IsDirty = true;
        }

        #endregion save

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
            Save();
        }

        public void TestButtonExecuted()
        {

            if (_controllerManager.Controllers ==null)
            {
                MessageBox.Show("null");
            }
            ControllerReport report = new ControllerReport(_controllerManager.Controllers);
            report.ShowDialog();

        }

        public void AddControllerExecuted()
        {
            _controllerManager.AddController("Controller", 16);
        }

        public bool CanExecuteIfCanAddControllers()
        {
            return _controllerManager.CanAddControllers;
        }

        public bool CanExecuteAlways()
        {
            return true;
        }

        public void DroppedItemExecuted(DragDropParameters parameters)
        {
            //SwapChannels(parameters.SourceOutput, parameters.DestnationOutput);
            Messenger.Instance.SendVixenChannelSwapRequest(parameters.SourceOutput, parameters.DestnationOutput);
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

        #region dragdrop events

        private void Expander_DragOver(object sender, DragEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                IController controller = item.DataContext as IController;
                if (controller != null)
                    controller.IsExpanded = true;
            }
        }

        private void ListboxOutputs_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("OutputDragItem"))
            {
                DragDropParameters dragDropData = e.Data.GetData("OutputDragItem") as DragDropParameters;
                //get what item we are dropping on
                var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
                if (item != null)
                {
                    //we're dropping onto a current item
                    dragDropData.DestnationOutput = item.DataContext as IControllerOutput;
                    IIsDropable viewModel = this.DataContext as IIsDropable;
                    if (viewModel != null)
                    {
                        if (viewModel.DroppedItem.CanExecute(null))
                        {
                            viewModel.DroppedItem.Execute(dragDropData);
                        }
                    }
                }
                else
                    MessageBox.Show("item is null");

                //OutputList.UnselectAll();
            }
        }

        #endregion dragdrop events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
