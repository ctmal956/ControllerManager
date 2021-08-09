using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControllerManager.Interfaces;

namespace ControllerManager.UserControls
{
    /// <summary>
    /// Interaction logic for ControllerControl.xaml
    /// </summary>
    public partial class ControllerDetailControl : UserControl
    {
        Point _dragStart = new Point();
        DragDropParameters _dragDropParameter = new DragDropParameters();
        ListBoxItem _dragListBoxItem;
        public ControllerDetailControl()
        {
            InitializeComponent();
        }

        private void OutputList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                // ListBox item clicked - do some cool things here
                _dragStart = e.GetPosition(this.OutputList);
                _dragListBoxItem = item;
                //e.Handled = true;
            }           
        }

        private void OutputList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(this.OutputList);
            Vector difference = currentPosition - _dragStart;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(difference.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(difference.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                //get the item that is selected.
                //ControllerOutput selectedOutput = (ControllerOutput)this.OuputList.SelectedItem;
                _dragDropParameter.SourceOutput = _dragListBoxItem.DataContext as IControllerOutput;
                DataObject dragData = new DataObject("OutputDragItem", _dragDropParameter);
                DragDrop.DoDragDrop(this.OutputList, dragData, DragDropEffects.Move);
            }
        }

        private void OutputList_Drop(object sender, DragEventArgs e)
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

                //OutputList.UnselectAll();
            }
            
        }
    }
}
