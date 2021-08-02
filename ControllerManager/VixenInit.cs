using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Xml;
using Vixen;

namespace ControllerManager
{
    public class VixenInit : IAddIn
    {
        private XmlNode _dataNode;
        
        private MainDisplay _display;
        private MainDisplayViewModel _displayViewModel;

        public LoadableDataLocation DataLocationPreference
        {
            get
            {
                return LoadableDataLocation.Sequence;
            }
        }

        public string Name => "Controller Manager";

        public string Author => "Chris Maloney";

        public string Description => "Manage your controllers and channels";

        public bool Execute(EventSequence sequence)
        {
            if (sequence == null)
            {
                MessageBox.Show("Please open a sequence before using this program.");
                return false;
            }
            else
            {
                

                //TestProfileOutputs(sequence);
                //return true;
                
                //notes, just cloning for now, if you save, you'll have to add code here to load deviceObjects
                
                _display = new MainDisplay();
                ElementHost.EnableModelessKeyboardInterop(_display);
                _displayViewModel = new MainDisplayViewModel(sequence, _dataNode) ;
                _display.DataContext = _displayViewModel;
                _display.Show();
                return true;
            }
            
        }

        public void Loading(XmlNode dataNode)
        {
            _dataNode = dataNode;
        }

        public void Unloading()
        {
            
        }

        

        

        private void TestProfileOutputs(EventSequence sequence)
        {
            sequence.Profile.MoveChannelObject(1, 0);
        }


        //This is the code to save the outputs correctly
        //private void buttonApply_Click(object sender, EventArgs e)
        //{
        //    List<Channel> newOutputList = new List<Channel>();
        //    foreach (ListViewItem lvi in listViewOutputs.Items)
        //    {
        //        newOutputList.Add(lvi.Tag as Channel);
        //    }
        //    if (_hasProfile)
        //    {
        //        _sequence.Profile.OutputChannels = newOutputList;
        //        AutoBackupProfile();
        //        _sequence.Profile.SaveToFile();
        //        _sequence.Profile.Reload();
        //        _recentLogCount = 0;
        //    }
        //    else
        //    {
        //    }

        //    ReloadListViewChannels();
        //    IsDirty = true;
        //}
    }
}
