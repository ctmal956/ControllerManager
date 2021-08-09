using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCommon;
using Vixen;
using System.Collections.ObjectModel;

namespace ControllerManager.ViewModel
{
    public class ControllerLabelViewModel:ViewModelBase
    {
        private IController _controller;
        public ControllerLabelViewModel(IController controller)
        {
            _controller = controller;
        }

        public string Name
        {
            get { return _controller.Name; }
        }

        public string Location
        {
            get { return _controller.Location; }
        }

        public ObservableCollection<IControllerOutput> LeftOutputs
        {
            get
            {
                int halfCount = _controller.Outputs.Count / 2;
                return GetRange(_controller.Outputs, 0, halfCount);
            }
        }

        public ObservableCollection<IControllerOutput> RightOutputs
        {
            get
            {
                int halfCount = _controller.Outputs.Count / 2;
                return GetRange(_controller.Outputs, halfCount, _controller.Outputs.Count - halfCount);
            }
        }

        private ObservableCollection<IControllerOutput> GetRange(ObservableCollection<IControllerOutput> controllers, int startIndex, int count)
        {
            var range = new ObservableCollection<IControllerOutput>();
            for (int i = startIndex; i < startIndex + count; i++)
                range.Add(controllers[i]);
            return range;
        }

    }
}
