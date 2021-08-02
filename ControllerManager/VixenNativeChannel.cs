using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Vixen;
using WPFCommon;

namespace ControllerManager
{
    public class VixenNativeChannel : ViewModelBase, IVixenChannel
    {
        Point _dragStartPoint;
        public VixenNativeChannel(Channel vixenOutput)
        {
            VixenChannel = vixenOutput;
        }

        private Channel _vixenChannel;
        public Channel VixenChannel
        {
            get { return _vixenChannel; }
            set
            {
                _vixenChannel = value;
                OnPropertyChanged("Name");
            }
        }

        public string Name
        {
            get { return VixenChannel.Name; }
            set
            {

            }
        }

        public int Output
        {
            get { return VixenChannel.OutputChannel;}
            set
            {

            }
        }

        public bool IsEnabled
        {
            get { return VixenChannel.Enabled; }
        }


        


    }
}
