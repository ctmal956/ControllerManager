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
using System.Windows.Shapes;
using ControllerManager.Interfaces;

namespace ControllerManager
{
    /// <summary>
    /// Interaction logic for MainDisplay.xaml
    /// </summary>
    public partial class MainDisplay : Window
    {
        public MainDisplay()
        {
            InitializeComponent();
        }

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

                //OutputList.UnselectAll();
            }
        }
    }
}
