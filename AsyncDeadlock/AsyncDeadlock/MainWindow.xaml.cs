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

namespace AsyncDeadlock
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lbMessage.Content = "Click begin";
            //var str = DoWork1().ConfigureAwait(false);
            var str = FDMTServiceWrapper().GetAwaiter().GetResult();
            //Same issue with GetAwaiter()
            lbMessage.Content = str;
            
        }

        public async Task<string> CoreService()
        {
            lbMessage.Content = "aaaa";
            var context = SynchronizationContext.Current;
            await Task.Run(() =>
            {
                for (var i = 0; i <= 2; i++)
                {
                    Thread.Sleep(1000);
                }
            }).ConfigureAwait(false);//tofix deadlock
            
            return "done";
        }
        public async Task<string> FDMTServiceWrapper()
        {
            //Can use this way
            //SynchronizationContext.SetSynchronizationContext(null);
            string test = "111";
            var context = SynchronizationContext.Current;
            var result = await CoreService().ConfigureAwait(false);
            string test2 = test;
            //lbMessage.Content = "aaaa";
            return result;
        }
    }
}
