using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.user_controll
{
    /// <summary>
    /// Interaction logic for sidebar.xaml
    /// </summary>
    public partial class sidebar : UserControl
    {
        public sidebar()
        {
            InitializeComponent();
        }

        private void NavigateToPage(string pageName)
        {
            try
            {
                // Get the main window
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    // Navigate to the page
                    mainWindow.MainFrame.Navigate(new Uri($"/pages/{pageName}.xaml", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation Error: {ex.Message}");
            }
        }

        private void NavigateToHome(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Home");
        }

        private void NavigateToTasks(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Tasks");
        }

        private void NavigateToCharts(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Charts");
        }

        private void NavigateToTimer(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Timer");
        }
    }
}
