using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "Click Started";
            DoWork();
            textBlock2.Text = "Click Finished";
        }
        private async void btnAsync_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "Click Started";
            await DoWork1();
            textBlock2.Text = "Click Finished";
        }
        private void btnReset_Click1(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "txt";
            textBlock2.Text = "txt";
        }
        private void Button_ClickXXX(object sender, RoutedEventArgs e)
        {
            Window currentWindow = null;
            DependencyObject owner = null;
            if (owner == null || owner is Window) currentWindow = owner as Window;
            else currentWindow = Window.GetWindow(owner);
        }

        void DoWork()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(300);
            }
        }

        public async Task DoWork1()
        {
            await Task.Run(() =>
            {
                for (var i = 0; i <= 10; i++)
                {
                    Thread.Sleep(300);
                }
            });
           
        }
    }
}
