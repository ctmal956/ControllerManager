using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using WPFCommon;
using ControllerManager.Interfaces;
using Vixen;

namespace ControllerManager
{
    public class ControllerManager:ViewModelBase
    {
        private ObservableCollection<IDisplayAbleObject> _displayItems;
        private ObservableCollection<IDisplayAbleObject> _channels;
        private ObservableCollection<IController> _controllers = new ObservableCollection<IController>();
        private bool _canAddControllers = true;
        private Messenger messenger = Messenger.Instance;
        
        

        public ControllerManager() : this(new ObservableCollection<IDisplayAbleObject>(), new ObservableCollection<IDisplayAbleObject>()){}

        public ControllerManager(ObservableCollection<IDisplayAbleObject> channels, ObservableCollection<IDisplayAbleObject> displayItems)
        {
            _displayItems = displayItems;
            _channels = channels;
            CanAddControllers = true;
            messenger.VixenChannelSwapRequested += Messenger_VixenChannelSwapRequested;
            IsChannelOrderDirty = false;
        }

        private void Messenger_VixenChannelSwapRequested(object sender, VixenChannelSwapRequestedEventArgs e)
        {
            SwapVixenChannels(e.Source, e.Destination);
        }

        public ObservableCollection<IDisplayAbleObject> DisplayItems { get; set; }
        public ObservableCollection<IDisplayAbleObject> Channels { get; set; }

        public bool CanAddControllers
        {
            get { return _canAddControllers; }
            private set
            {
                _canAddControllers = value;
                OnPropertyChanged("CanAddControllers");
            }
        }

        public ObservableCollection<IController> Controllers
        {
            get 
            {
                var controllers = new ObservableCollection<IController>();
                foreach (var item in DisplayItems)
                {
                    IController controller = item as IController;
                    if (controller != null)
                    {
                        controllers.Add(controller);
                    }
                }
                return controllers;
            }
            set
            {
                OnPropertyChanged("Controllers");
            }
        }

        public void AddController(String nameBase, int channelCount)
        {
            int nextControllerIndex = GetControllerCount();
            string controllerName = nameBase + " " + (nextControllerIndex + 1).ToString();
            IController newController = new Controller(controllerName, channelCount);

            AssignVixenChannels(newController);

            //add the new controller to the list
            DisplayItems.Insert(nextControllerIndex, newController);

            CanAddControllers = GetTotalControllerChannelCount() < Channels.Count ? true : false;
        }

        public void AddController(IController controller)
        {
            int nextControllerIndex = GetControllerCount();
            AssignVixenChannels(controller);
            DisplayItems.Insert(nextControllerIndex, controller);
            CanAddControllers = GetTotalControllerChannelCount() < Channels.Count ? true : false;
        }

        /// <summary>
        /// assigns the next available channel item in displayItems and removes it from the list
        /// </summary>
        /// <param name="controller"></param>
        private void AssignVixenChannels(IController controller)
        {
            int nextChannelIndex = GetControllerCount();
            foreach (IControllerOutput output in controller.Outputs)
            {
                if (nextChannelIndex < DisplayItems.Count())
                {
                    output.VixenChannel = DisplayItems[nextChannelIndex] as IVixenChannel;
                    DisplayItems.RemoveAt(nextChannelIndex);
                }
            }
        }

        private int GetFirstChannelIndex()
        {
            return GetTotalControllerChannelCount() - 1;
        }

        private int GetTotalControllerChannelCount()
        {
            int channelCount = 0;
            foreach (IDisplayAbleObject item in DisplayItems)
            {
                IController controller = item as IController;
                if (controller != null)
                {
                    channelCount += controller.ChannelCount;
                }
                else
                {
                    return channelCount;
                }
            }
            return channelCount;
        }

        private int GetControllerCount()
        {
            int controllerCount = 0;
            foreach (IDisplayAbleObject item in DisplayItems)
            {
                IController controller = item as IController;
                if (controller != null)
                {
                    controllerCount ++;
                }
                else
                {
                    return controllerCount;
                }
            }
            return controllerCount;
        }


        private void SwapVixenChannels(IControllerOutput source, IControllerOutput destination)
        {
            if (source != null && destination != null)
            {
                IVixenChannel tmp = source.VixenChannel;
                source.VixenChannel = destination.VixenChannel;
                destination.VixenChannel = tmp;
                IsChannelOrderDirty = true;
            }
        }

        private void UpdateAssignedChannels()
        {

        }

        public List<Channel> GetNewOutputList()
        {
            var channelList = new List<Channel>();
            foreach (IDisplayAbleObject item in this.DisplayItems)
            {
                var controller = item as IController;
                if (controller != null)
                {
                    foreach (ControllerOutput output in controller.Outputs)
                    {
                        if (output.VixenChannel.VixenChannel != null)
                        {
                            channelList.Add(output.VixenChannel.VixenChannel);
                        }
                        
                    }
                }
                else
                {
                    channelList.Add((item as IVixenChannel).VixenChannel);
                }
            }
            return channelList;
        }

        private bool _isChannelOrderDirty;

        public bool IsChannelOrderDirty
        {
            get { return _isChannelOrderDirty; }
            set
            {
                _isChannelOrderDirty = value;
                OnPropertyChanged("IsChannelOrderDirty");
            }
        }


    }
}
