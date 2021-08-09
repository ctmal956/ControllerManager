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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControllerManager.Interfaces;
using ControllerManager.UserControls;
using ControllerManager.ViewModel;

namespace ControllerManager
{
    /// <summary>
    /// Interaction logic for ControllerReport.xaml
    /// </summary>
    public partial class ControllerReport : Window
    {
        private ObservableCollection<IController> _controllers = new ObservableCollection<IController>();
        private FixedDocument doc = new FixedDocument();
        
        public ControllerReport(ObservableCollection<IController> controllers)
        {
            InitializeComponent();
            _controllers = controllers;
            AddControllersToPage();
            DocViewer.Document = doc;
        }

        private void AddControllersToPage()
        {
            foreach (var controller in _controllers)
            {
                doc.Pages.Add(CreatePage(controller));
            }
        }

        private PageContent CreatePage(IController controller)
        {
            PageContent content = new PageContent();
            FixedPage newPage = new FixedPage();
            //newPage.Children.Add(SetTitle(controller.Name));
            //(content as IAddChild).AddChild(newPage);
            ControllerReportTemplate template = new ControllerReportTemplate();
            ControllerLabelViewModel vm = new ControllerLabelViewModel(controller);
            template.DataContext = vm;
            template.UpdateLayout();
            newPage.Children.Add(template);
            (content as IAddChild).AddChild(newPage);


            return content;
        }

        private TextBlock SetTitle(string title)
        {
            TextBlock tb = new TextBlock();
            tb.Text = title;
            tb.Style = Resources["Title"] as Style;
            return tb;
        }

        
    }
}
