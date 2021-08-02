using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using WPFCommon;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    public class ControllerManager:ViewModelBase
    {
        private ObservableCollection<IDisplayAbleObject> _displayItems;
        private ObservableCollection<IDisplayAbleObject> _channels;
        private bool _canAddControllers = true;
        private Messenger messenger = Messenger.Instance;

        public ControllerManager() : this(new ObservableCollection<IDisplayAbleObject>(), new ObservableCollection<IDisplayAbleObject>()){}

        public ControllerManager(ObservableCollection<IDisplayAbleObject> channels, ObservableCollection<IDisplayAbleObject> displayItems)
        {
            _displayItems = displayItems;
            _channels = channels;
            CanAddControllers = true;
            messenger.VixenChannelSwapRequested += Messenger_VixenChannelSwapRequested;
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

        public void AddController(String nameBase, int channelCount)
        {
            int nextControllerIndex = GetControllerCount();
            int nextChannelIndex = GetTotalControllerChannelCount();
            IController newController = new Controller();
            newController.Name = nameBase + " " + (nextControllerIndex + 1).ToString();

            //if (GetTotalControllerChannelCount() + channelCount > Channels.Count)
            //{
            //    newController.ChannelCount = Channels.Count - GetTotalControllerChannelCount();
            //}
            //else
            //{
            //    newController.ChannelCount = channelCount;
            //}

            newController.ChannelCount = channelCount;

            for (int index = 0; index < newController.ChannelCount; index++)
            {
                IVixenChannel channel = (nextChannelIndex + index) < this.Channels.Count ? this.Channels[nextChannelIndex + index] as IVixenChannel:null;
                newController.Outputs.Add(new ControllerOutput(channel, index + 1));   
            }

            //add the new controller to the list
            DisplayItems.Insert(nextControllerIndex, newController);

            //remove the appropriate channels from the list
            for (int index = 0; index < newController.ChannelCount; index++)
            {
                if ((nextChannelIndex + index) < this.Channels.Count)
                    DisplayItems.RemoveAt(nextControllerIndex+1);
            }

            CanAddControllers = GetTotalControllerChannelCount() < Channels.Count ? true : false;
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
            }
        }

    }
}
