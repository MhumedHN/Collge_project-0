using System;
using System.Windows;
using System.Windows.Navigation;
using UI.pages;
using UI.user_controll; 
namespace UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Uri("/pages/Home.xaml", UriKind.Relative));
        }
    }
}
